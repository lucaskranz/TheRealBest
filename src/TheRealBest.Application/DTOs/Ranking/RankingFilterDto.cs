namespace TheRealBest.Application.DTOs.Ranking;

using TheRealBest.Domain.Enums;

public sealed record RankingFilterDto(
    int SeasonYear = 2023,
    PlayerPosition? Position = null,
    string? Nationality = null,
    bool OnlyEligible = true,
    int Page = 1,
    int PageSize = 25
);