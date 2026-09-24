namespace TheRealBest.Application.DTOs.Audit;

public sealed record AuditActionItemDto(
    string ActionKey,
    string Label,
    decimal Count,
    decimal UnitWeight,
    decimal TotalPoints,
    int? Minute = null,
    decimal MinutesFactor = 1.0m
);