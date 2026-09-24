namespace TheRealBest.Application.DTOs.Players;

public sealed record PlayerSummaryDto(
    Guid Id,
    string Name,
    string Nationality,
    string PrimaryPosition,
    string? PhotoUrl,
    DateOnly? DateOfBirth
);