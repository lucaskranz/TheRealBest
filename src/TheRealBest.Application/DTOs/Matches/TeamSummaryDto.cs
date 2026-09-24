namespace TheRealBest.Application.DTOs.Matches;

public sealed record TeamSummaryDto(
    Guid Id,
    string Name,
    string ShortName,
    string LogoUrl,
    string Country,
    int EloRanking
);