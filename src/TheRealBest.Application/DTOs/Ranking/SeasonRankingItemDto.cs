namespace TheRealBest.Application.DTOs.Ranking;

public sealed record SeasonRankingItemDto(
    Guid PlayerId,
    string PlayerName,
    string Nationality,
    string? PhotoUrl,
    string PrimaryPosition,
    string PrimaryPositionLabel,
    string? TeamName,
    string? TeamLogoUrl,
    int OverallRank,
    int PositionRank,
    decimal FssScore,
    decimal MpsAverage,
    decimal PresenceFactor,
    decimal ClutchIndex,
    int TotalMatches,
    int TotalMinutes,
    bool IsRankingEligible,
    DateTime RecalculatedAt,
    IReadOnlyList<Top5MatchItemDto> TopMatches
);