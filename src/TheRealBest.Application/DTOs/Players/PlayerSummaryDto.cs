namespace TheRealBest.Application.DTOs.Players;

public sealed record PlayerSummaryDto(
    Guid Id,
    string Name,
    string Nationality,
    string PrimaryPosition,
    string PrimaryPositionLabel,
    string? PhotoUrl,
    DateOnly? DateOfBirth
);