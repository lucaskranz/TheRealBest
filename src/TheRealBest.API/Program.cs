using TheRealBest.API.BackgroundServices;
using TheRealBest.API.Commands;
using TheRealBest.API.Formula;
using TheRealBest.API.Localization;
using TheRealBest.API.Middleware;
using TheRealBest.Application;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure;
using TheRealBest.Infrastructure.Data.Seeds;
using TheRealBest.Infrastructure.Ingestion;
using TheRealBest.Scoring;
using TheRealBest.Scoring.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IScoringRulesProvider, ScoringRulesProvider>();
builder.Services.AddSingleton<IScoringEngine, ScoringEngine>();
builder.Services.AddSingleton<ITranslationService, ResxTranslationService>();
builder.Services.AddScoped<FormulaDescriptorFactory>();

// CORS for Frontend (Next.js)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Background Services
builder.Services.AddHostedService<MatchDataIngestionService>();
builder.Services.AddHostedService<RankingRecalculationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Localização primeiro: a cultura precisa valer também para as mensagens de erro
app.UseMiddleware<LocalizationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed com dados reais da API-Football: só sob demanda (consome cota da API), e encerra ao terminar.
// Uso: dotnet run --project src/TheRealBest.API -- --seed
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<IRealDataSeeder>().SeedAsync();
    return;
}

// Importação de uma temporada inteira (ou de uma competição dela). Retomável; consome cota da API.
// Uso: dotnet run --project src/TheRealBest.API -- --import-season 2023 [--competition 9] [--limit 5]
if (ImportSeasonCommand.IsRequested(args))
{
    var request = ImportSeasonCommand.Parse(args, out var error);
    if (request is null)
    {
        Console.Error.WriteLine(error);
        Environment.ExitCode = 1;
        return;
    }

    using var scope = app.Services.CreateScope();
    var summary = await scope.ServiceProvider.GetRequiredService<ISeasonImporter>().ImportAsync(request);
    Environment.ExitCode = summary.Stop is SeasonImportStop.SeasonNotInPlan ? 1 : 0;
    return;
}

app.Run();
public partial class Program { }
