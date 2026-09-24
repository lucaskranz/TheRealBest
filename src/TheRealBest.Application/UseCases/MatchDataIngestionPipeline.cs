namespace TheRealBest.Application.UseCases;

using Microsoft.Extensions.Logging;
using TheRealBest.Application.DTOs;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Interfaces;

public sealed class MatchDataIngestionPipeline(
    IFootballDataProvider dataProvider,
    IIngestMatchDataUseCase ingestUseCase,
    IRecalculateSeasonRankingUseCase recalculateUseCase,
    ILogger<MatchDataIngestionPipeline> logger) : IMatchDataIngestionPipeline
{
    public async Task<IReadOnlyList<IngestionResultDto>> IngestCompetitionSeasonAsync(
        string competitionExternalId,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Iniciando pipeline de ingestÃ£o: CompetiÃ§Ã£o {CompetitionId}, Temporada {SeasonYear}",
            competitionExternalId, seasonYear);

        var fixtures = await dataProvider.GetFixturesAsync(competitionExternalId, seasonYear, cancellationToken);
        var finishedFixtures = fixtures.Where(f => f.IsFinished).ToList();

        logger.LogInformation("Encontradas {Total} partidas ({Finished} finalizadas) para ingestÃ£o.",
            fixtures.Count, finishedFixtures.Count);

        var results = new List<IngestionResultDto>();

        foreach (var fixture in finishedFixtures)
        {
            if (cancellationToken.IsCancellationRequested) break;

            try
            {
                var report = await dataProvider.GetMatchReportAsync(fixture, cancellationToken);
                var result = await ingestUseCase.ExecuteAsync(report, cancellationToken);
                results.Add(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao ingerir partida {FixtureId}. Continuando prÃ³ximas.", fixture.ExternalId);
            }
        }

        // Recalcular rankings apÃ³s lote de importaÃ§Ã£o
        if (results.Any(r => !r.AlreadyExists))
        {
            await recalculateUseCase.ExecuteAsync(seasonYear, cancellationToken);
        }

        return results;
    }
}