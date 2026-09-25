namespace TheRealBest.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Refit;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.Data;
using TheRealBest.Infrastructure.Ingestion;
using TheRealBest.Infrastructure.Data.Repositories;
using TheRealBest.Infrastructure.Data.Seeds;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Infrastructure.ExternalApis.ClubElo;

public static class DependencyInjection
{
    public const string ConnectionStringName = "DefaultConnection";
    public const string MigrationsHistoryTable = "__ef_migrations_history";
    public const string ApiFootballHttpClientName = "ApiFootball";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<AppDbContext>(options => ConfigureDbContext(options, connectionString));
        services.AddRepositories();
        services.AddApiFootball(configuration);
        services.AddClubElo(configuration);
        services.AddScoped<IRealDataSeeder, ApiFootballRealDataSeeder>();
        services.AddScoped<ISeasonImporter, SeasonImporter>();
        services.AddScoped<ISeasonAuditor, SeasonAuditor>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ICompetitionRepository, CompetitionRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IRankingRepository, RankingRepository>();
        return services;
    }

    public static DbContextOptionsBuilder ConfigureDbContext(DbContextOptionsBuilder options, string connectionString) =>
        options
            .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsql.MigrationsHistoryTable(MigrationsHistoryTable);
                npgsql.EnableRetryOnFailure(maxRetryCount: 3);
            })
            .UseSnakeCaseNamingConvention();

    private static void AddClubElo(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(ClubEloOptions.SectionName).Get<ClubEloOptions>() ?? new ClubEloOptions();
        services.AddHttpClient(ClubEloProvider.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });

        // Singleton: o cache em memória dos ratings por data vale para toda a importação
        services.AddSingleton<IClubEloProvider>(provider => new ClubEloProvider(
            provider.GetRequiredService<IHttpClientFactory>().CreateClient(ClubEloProvider.HttpClientName),
            options,
            provider.GetRequiredService<ILogger<ClubEloProvider>>()));
    }

    private static void AddApiFootball(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiFootballOptions>(configuration.GetSection(ApiFootballOptions.SectionName));
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<ApiFootballRequestPacer>();
        services.AddTransient<ApiFootballResponseCacheHandler>();
        services.AddTransient<ApiFootballRateLimitHandler>();
        services.AddTransient<ApiFootballAuthHandler>();

        var refitSettings = new RefitSettings(new SystemTextJsonContentSerializer(ApiFootballJson.Options));

        // Ordem dos handlers: retry (externo) → cache em disco → pacer → autenticação.
        // Acertos no cache não consomem cota; cada nova tentativa também respeita o ritmo.
        services.AddRefitClient<IApiFootballApi>(_ => refitSettings, ApiFootballHttpClientName)
            .ConfigureHttpClient((provider, client) =>
                client.BaseAddress = new Uri(provider.GetRequiredService<IOptions<ApiFootballOptions>>().Value.BaseUrl))
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
            .AddHttpMessageHandler<ApiFootballResponseCacheHandler>()
            .AddHttpMessageHandler<ApiFootballRateLimitHandler>()
            .AddHttpMessageHandler<ApiFootballAuthHandler>();

        services.AddScoped<IFootballDataProvider, ApiFootballDataProvider>();
    }
}