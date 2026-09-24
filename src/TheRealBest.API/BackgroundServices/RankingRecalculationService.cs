namespace TheRealBest.API.BackgroundServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.Interfaces;

/// <summary>
/// Worker em background para recÃ¡lculo periÃ³dico do Fair Season Score (FSS).
/// </summary>
public sealed class RankingRecalculationService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<RankingRecalculationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = configuration.GetValue<bool>("RankingRecalculation:Enabled", defaultValue: false);
        if (!enabled)
        {
            logger.LogInformation("RankingRecalculationService desativado na configuraÃ§Ã£o (RankingRecalculation:Enabled=false).");
            return;
        }

        var intervalMinutes = configuration.GetValue<int>("RankingRecalculation:IntervalMinutes", defaultValue: 360);
        logger.LogInformation("RankingRecalculationService iniciado com intervalo de {Interval} minutos.", intervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var recalculateUseCase = scope.ServiceProvider.GetRequiredService<IRecalculateSeasonRankingUseCase>();

                var seasonYear = configuration.GetValue<int>("RankingRecalculation:DefaultSeasonYear", defaultValue: 2023);
                logger.LogInformation("Iniciando recÃ¡lculo do ranking da temporada {SeasonYear}...", seasonYear);

                var summary = await recalculateUseCase.ExecuteAsync(seasonYear, stoppingToken);
                logger.LogInformation("Ranking recalculado com sucesso. {Eligible} jogadores elegÃ­veis de {Total} avaliados.",
                    summary.EligibleCount, summary.TotalRanked);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Erro no recÃ¡lculo periÃ³dico do ranking.");
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

        logger.LogInformation("RankingRecalculationService finalizado.");
    }
}