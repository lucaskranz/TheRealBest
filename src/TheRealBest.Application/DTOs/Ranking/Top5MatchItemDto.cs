namespace TheRealBest.Application.DTOs.Ranking;

public sealed record Top5MatchItemDto(
    Guid MatchId,
    DateTime Date,
    string Phase,
    decimal Mps
);