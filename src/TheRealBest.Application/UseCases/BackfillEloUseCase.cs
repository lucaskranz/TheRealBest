namespace TheRealBest.Application.UseCases;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.DTOs;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// Preenche o Elo das partidas de clubes gravadas sem ele (ClubElo fora do ar na importação ou clube sem correspondência)
/// e recalcula as notas dessas partidas a partir das estatísticas gravadas. Não consome cota da API-Football.
/// Idempotente: partidas com os dois ratings não são tocadas, e as que continuam incompletas são tentadas de novo na próxima execução.
/// </summary>
public sealed class BackfillEloUseCase(
    IMatchRepository matchRepository,
    IUnitOfWork unitOfWork,
    IScoringEngine scoringEngine,
    IClubEloProvider clubEloProvider,
    IRecalculateSeasonRankingUseCase recalculateUseCase,
    ILogger<BackfillEloUseCase> logger) : IBackfillEloUseCase
{
    /// <summary>Partidas carregadas e gravadas por vez.</summary>
    public const int ChunkSize = 50;

    public async Task<EloBackfillSummaryDto> ExecuteAsync(int seasonYear, CancellationToken cancellationToken = default)
    {
        var matchIds = await matchRepository.GetClubMatchIdsMissingEloAsync(seasonYear, cancellationToken);
        logger.LogInformation("Temporada {Season}: {Count} partidas de clubes sem Elo completo.", seasonYear, matchIds.Count);

        var checkedCount = 0;
        var updated = 0;
        var rescored = 0;
        var notFound = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var chunk in matchIds.Chunk(ChunkSize))
        {
            foreach (var match in await matchRepository.GetForRescoringAsync(chunk, cancellationToken))
            {
                checkedCount++;
                var date = DateOnly.FromDateTime(match.MatchDate);
                var home = match.HomeEloRating ?? await clubEloProvider.GetEloAsync(match.HomeTeam.Name, date, cancellationToken);
                var away = match.AwayEloRating ?? await clubEloProvider.GetEloAsync(match.AwayTeam.Name, date, cancellationToken);

                if (home is null) notFound.Add(match.HomeTeam.Name);
                if (away is null) notFound.Add(match.AwayTeam.Name);

                if (home == match.HomeEloRating && away == match.AwayEloRating)
                {
                    continue;
                }

                match.SetEloRatings(home, away);
                rescored += Rescore(match);
                updated++;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            unitOfWork.ClearTracking();
            logger.LogInformation("Elo: {Done}/{Total} partidas verificadas, {Updated} atualizadas.", checkedCount, matchIds.Count, updated);
        }

        if (updated > 0)
        {
            await recalculateUseCase.ExecuteAsync(seasonYear, cancellationToken);
        }

        return new EloBackfillSummaryDto(seasonYear, matchIds.Count, updated, rescored, notFound.ToList());
    }

    private int Rescore(Match match)
    {
        var scoresByStats = match.PerformanceScores.ToDictionary(s => s.MatchPlayerStatsId);
        var count = 0;

        foreach (var stats in match.PlayerStats)
        {
            if (!scoresByStats.TryGetValue(stats.Id, out var score))
            {
                continue;
            }

            var receipt = scoringEngine.CalculateMatchScore(stats, MatchContext.From(match, stats.TeamId));
            score.ApplyReceipt(receipt, JsonSerializer.Serialize(receipt.ActionBreakdown), JsonSerializer.Serialize(receipt.PenaltyBreakdown));
            count++;
        }

        return count;
    }
}
