namespace TheRealBest.Application.DTOs.Matches;

public sealed record MatchDetailDto(
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
    IReadOnlyList<MatchPlayerPerformanceDto> PlayerPerformances
);