namespace TheRealBest.Application.DTOs.Players;

using TheRealBest.Application.DTOs.Ranking;

public sealed record PlayerProfileDto(
    Guid Id,
    string Name,
    string Nationality,
    string PrimaryPosition,
    string PrimaryPositionLabel,
    string? PhotoUrl,
    DateOnly? DateOfBirth,
    SeasonRankingItemDto? SeasonRanking,
    IReadOnlyList<PlayerMatchStatDto> RecentMatches
);