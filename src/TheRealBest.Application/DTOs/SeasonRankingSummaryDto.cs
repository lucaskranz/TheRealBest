namespace TheRealBest.Application.DTOs;

public sealed record SeasonRankingSummaryDto(
    int SeasonYear,
    int TotalRanked,
    int EligibleCount,
    DateTime RecalculatedAt
);