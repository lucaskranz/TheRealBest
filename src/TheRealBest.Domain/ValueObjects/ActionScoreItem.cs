namespace TheRealBest.Domain.ValueObjects;

/// <summary>
/// Item detalhado do extrato auditÃ¡vel de pontuaÃ§Ã£o da partida.
/// </summary>
public sealed record ActionScoreItem(
    string ActionKey,
    string Label,
    int Count,
    decimal UnitWeight,
    decimal TotalPoints,
    int? Minute = null
);