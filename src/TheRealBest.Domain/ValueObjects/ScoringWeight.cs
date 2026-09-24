namespace TheRealBest.Domain.ValueObjects;

using TheRealBest.Domain.Enums;

/// <summary>
/// Representa o peso posicional de uma aÃ§Ã£o estatÃ­stica.
/// </summary>
public sealed record ScoringWeight(
    PlayerPosition Position,
    ActionType ActionType,
    decimal WeightValue,
    bool IsPenalty,
    string Description
);