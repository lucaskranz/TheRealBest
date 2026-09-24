namespace TheRealBest.Application.UseCases;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.DTOs;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;

public sealed class RecalculateSeasonRankingUseCase(
    IMatchRepository matchRepository,
    IRankingRepository rankingRepository,
    IScoringEngine scoringEngine,
    ILogger<RecalculateSeasonRankingUseCase> logger) : IRecalculateSeasonRankingUseCase
{
    public async Task<SeasonRankingSummaryDto> ExecuteAsync(int seasonYear, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Recalculando ranking da temporada {SeasonYear}...", seasonYear);

        var scores = await matchRepository.GetSeasonScoresAsync(seasonYear, cancellationToken);

        if (scores.Count == 0)
        {
            logger.LogWarning("Nenhum score encontrado para a temporada {SeasonYear}.", seasonYear);
            return new SeasonRankingSummaryDto(seasonYear, 0, 0, DateTime.UtcNow);
        }

        var playerGroups = scores.GroupBy(s => s.PlayerId).ToList();
        var rankingList = new List<SeasonRanking>();

        foreach (var group in playerGroups)
        {
            var playerId = group.Key;
            var player = group.First().Player;

            var playerScores = group.ToList();
            var totalSeasonMinutes = playerScores.Sum(s => s.MatchPlayerStats.MinutesPlayed);

            var seasonScore = scoringEngine.CalculateSeasonScore(playerScores, totalSeasonMinutes);

            var top5 = playerScores
                .OrderByDescending(s => s.FinalMps)
                .Take(5)
                .Select(s => new
                {
                    MatchId = s.MatchId,
                    Mps = s.FinalMps,
                    Date = s.Match.MatchDate,
                    Phase = s.Match.RoundPhase
                });

            var top5Json = JsonSerializer.Serialize(top5);
            var clutchIndex = playerScores.Count > 0 ? Math.Round(playerScores.Average(s => s.ClutchMultiplier), 3) : 1.000m;

            var ranking = new SeasonRanking(
                playerId,
                seasonYear,
                seasonScore.MatchesCounted,
                totalSeasonMinutes,
                seasonScore.MpsAverage,
                seasonScore.WeightedMpsSum,
                seasonScore.PresenceFactor,
                seasonScore.Fss,
                overallRank: 0,
                positionRank: 0,
                clutchIndex,
                seasonScore.IsRankingEligible,
                top5Json);

            rankingList.Add(ranking);
        }

        // Atribuir OverallRank para jogadores elegÃ­veis
        var eligiblePlayers = rankingList
            .Where(r => r.IsRankingEligible)
            .OrderByDescending(r => r.FssScore)
            .ToList();

        for (int i = 0; i < eligiblePlayers.Count; i++)
        {
            eligiblePlayers[i].UpdateRanks(i + 1, 0);
        }

        // Atribuir PositionRank por posiÃ§Ã£o para jogadores elegÃ­veis
        var positionGroups = eligiblePlayers
            .GroupBy(r => scores.First(s => s.PlayerId == r.PlayerId).Player.PrimaryPosition);

        foreach (var posGroup in positionGroups)
        {
            var sortedPos = posGroup.OrderByDescending(r => r.FssScore).ToList();
            for (int j = 0; j < sortedPos.Count; j++)
            {
                sortedPos[j].UpdateRanks(sortedPos[j].OverallRank, j + 1);
            }
        }

        await rankingRepository.BulkUpsertRankingsAsync(rankingList, cancellationToken);

        logger.LogInformation("Ranking da temporada {SeasonYear} recalculado. {Total} avaliados, {Eligible} elegÃ­veis no ranking oficial.",
            seasonYear, rankingList.Count, eligiblePlayers.Count);

        return new SeasonRankingSummaryDto(seasonYear, rankingList.Count, eligiblePlayers.Count, DateTime.UtcNow);
    }
}