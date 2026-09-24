namespace TheRealBest.Application.DTOs.Matches;

public sealed record MatchFilterDto(
    int SeasonYear = 2023,
    Guid? CompetitionId = null,
    Guid? TeamId = null,
    int Page = 1,
    int PageSize = 20
);