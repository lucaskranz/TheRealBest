namespace TheRealBest.Domain.ValueObjects;

using TheRealBest.Domain.Enums;

/// <summary>
/// Recibo completo e auditÃ¡vel da pontuaÃ§Ã£o da partida de um jogador.
/// </summary>
public sealed record MatchReceipt(
    Guid PlayerId,
    Guid MatchId,
    PlayerPosition PositionEvaluated,
    decimal BaseScore,
    IReadOnlyList<ActionScoreItem> ActionBreakdown,
    IReadOnlyList<ActionScoreItem> PenaltyBreakdown,
    decimal SubtotalRaw,
    ContextMultiplier ContextMultiplier,
    decimal MinutesFactor,
    decimal FinalMps,
    int AlgorithmVersion,
    DateTime CalculatedAt
);