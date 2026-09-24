using TheRealBest.API.BackgroundServices;
using TheRealBest.Application;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure;
using TheRealBest.Infrastructure.Data.Seeds;
using TheRealBest.Scoring;
using TheRealBest.Scoring.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IScoringRulesProvider, ScoringRulesProvider>();
builder.Services.AddSingleton<IScoringEngine, ScoringEngine>();

// Background Services
builder.Services.AddHostedService<MatchDataIngestionService>();
builder.Services.AddHostedService<RankingRecalculationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Executa o seed automÃ¡tico se a base de partidas estiver vazia ou com argumento --seed
var isSeedArg = args.Contains("--seed");
var shouldSeedOnStartup = builder.Configuration.GetValue<bool>("Seed:AutoSeedOnStartup", defaultValue: true);

if (isSeedArg || shouldSeedOnStartup)
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IRealDataSeeder>();
    await seeder.SeedAsync();
}

app.Run();