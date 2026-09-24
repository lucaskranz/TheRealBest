namespace TheRealBest.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Polly;
using Refit;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.Data;
using TheRealBest.Infrastructure.Data.Repositories;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

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

    private static void AddApiFootball(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiFootballOptions>(configuration.GetSection(ApiFootballOptions.SectionName));
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<ApiFootballRequestPacer>();
        services.AddTransient<ApiFootballRateLimitHandler>();
        services.AddTransient<ApiFootballAuthHandler>();

        var refitSettings = new RefitSettings(new SystemTextJsonContentSerializer(ApiFootballJson.Options));

        services.AddRefitClient<IApiFootballApi>(_ => refitSettings, ApiFootballHttpClientName)
            .ConfigureHttpClient((provider, client) =>
                client.BaseAddress = new Uri(provider.GetRequiredService<IOptions<ApiFootballOptions>>().Value.BaseUrl))
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
            .AddHttpMessageHandler<ApiFootballRateLimitHandler>()
            .AddHttpMessageHandler<ApiFootballAuthHandler>();

        services.AddScoped<IFootballDataProvider, ApiFootballDataProvider>();
    }
}