namespace TheRealBest.API.BackgroundServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.Interfaces;

/// <summary>
/// Worker em background que orquestra a ingestÃƒÂ£o contÃƒÂ­nua de partidas e cÃƒÂ¡lculo de notas.
/// Respeita o padrÃƒÂ£o de IHostedService e IServiceScopeFactory conforme rules do projeto.
/// </summary>
public sealed class MatchDataIngestionService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<MatchDataIngestionService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = configuration.GetValue<bool>("Ingestion:Enabled", defaultValue: false);
        if (!enabled)
        {
            logger.LogInformation("MatchDataIngestionService desativado na configuraÃƒÂ§ÃƒÂ£o (Ingestion:Enabled=false).");
            return;
        }

        var intervalMinutes = configuration.GetValue<int>("Ingestion:IntervalMinutes", defaultValue: 60);
        logger.LogInformation("MatchDataIngestionService iniciado com intervalo de {Interval} minutos.", intervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var pipeline = scope.ServiceProvider.GetRequiredService<IMatchDataIngestionPipeline>();

                logger.LogInformation("Executando ciclo periÃƒÂ³dico de ingestÃƒÂ£o de partidas...");
                // Exemplo: Champions League 2023/24 (ID externo 2)
                var competitionId = configuration.GetValue<string>("Ingestion:DefaultCompetitionExternalId") ?? "2";
                var seasonYear = configuration.GetValue<int>("Ingestion:DefaultSeasonYear", defaultValue: 2023);

                var results = await pipeline.IngestCompetitionSeasonAsync(competitionId, seasonYear, stoppingToken);
                logger.LogInformation("Ciclo de ingestÃƒÂ£o finalizado com {Count} partidas processadas.", results.Count);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Erro durante a execuÃƒÂ§ÃƒÂ£o do ciclo de ingestÃƒÂ£o de dados de partidas.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        logger.LogInformation("MatchDataIngestionService finalizado.");
    }
}