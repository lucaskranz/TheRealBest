namespace TheRealBest.Application.UseCases;

using System.Text.Json;
using TheRealBest.Application.DTOs.Audit;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;

public sealed class GetMatchAuditReceiptUseCase(IMatchRepository matchRepository) : IGetMatchAuditReceiptUseCase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<MatchPerformanceReceiptDto?> ExecuteAsync(Guid matchId, Guid playerId, CancellationToken cancellationToken = default)
    {
        var score = await matchRepository.GetPerformanceScoreAsync(matchId, playerId, cancellationToken);
        if (score is null) return null;

        var positiveActions = DeserializeBreakdown(score.ActionBreakdownJson);
        var penalties = DeserializeBreakdown(score.PenaltyBreakdownJson);

        var formula = new AuditFormulaDto(
            BaseScore: score.BaseScore,
            SubtotalRaw: score.SubtotalRaw,
            PositionBaseline: score.PositionBaseline,
            MinutesFactor: score.MinutesFactor,
            TournamentMultiplier: score.TournamentMultiplier,
            OpponentMultiplier: score.OpponentMultiplier,
            ClutchMultiplier: score.ClutchMultiplier,
            ContextMultiplierCombined: score.ContextMultiplierCombined,
            FinalMps: score.FinalMps,
            CountsTowardsSeason: score.CountsTowardsSeason
        );

        return new MatchPerformanceReceiptDto(
            ScoreId: score.Id,
            MatchId: score.MatchId,
            PlayerId: score.PlayerId,
            PlayerName: score.Player?.Name ?? string.Empty,
            PlayerPosition: score.PositionEvaluated.ToString(),
            PhotoUrl: score.Player?.PhotoUrl,
            MatchDate: score.Match?.MatchDate ?? DateTime.MinValue,
            CompetitionName: score.Match?.Competition?.Name ?? string.Empty,
            HomeTeamName: score.Match?.HomeTeam?.Name ?? string.Empty,
            AwayTeamName: score.Match?.AwayTeam?.Name ?? string.Empty,
            HomeScore: score.Match?.HomeScore,
            AwayScore: score.Match?.AwayScore,
            MinutesPlayed: score.MatchPlayerStats?.MinutesPlayed ?? 0,
            AlgorithmVersion: score.AlgorithmVersion,
            CalculatedAt: score.CalculatedAt,
            Formula: formula,
            PositiveActions: positiveActions,
            Penalties: penalties
        );
    }

    private static IReadOnlyList<AuditActionItemDto> DeserializeBreakdown(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];

        try
        {
            var items = JsonSerializer.Deserialize<List<ActionScoreItem>>(json, JsonOptions);
            if (items is null) return [];

            return items.Select(i => new AuditActionItemDto(
                ActionKey: i.ActionKey,
                Label: i.Label,
                Count: i.Count,
                UnitWeight: i.UnitWeight,
                TotalPoints: i.TotalPoints,
                Minute: i.Minute,
                MinutesFactor: i.MinutesFactor
            )).ToList();
        }
        catch
        {
            return [];
        }
    }
}