namespace TheRealBest.Application.DTOs.Matches;

public sealed record MatchSummaryDto(
    Guid Id,
    DateTime MatchDate,
    string CompetitionName,
    string CompetitionTier,
    string RoundPhase,
    bool IsKnockout,
    TeamSummaryDto HomeTeam,
    TeamSummaryDto AwayTeam,
    int? HomeScore,
    int? AwayScore,
    int EvaluatedPlayersCount
);