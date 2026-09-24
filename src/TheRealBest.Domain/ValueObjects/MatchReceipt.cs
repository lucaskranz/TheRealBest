namespace TheRealBest.Domain.ValueObjects;

using TheRealBest.Domain.Enums;

/// <summary>
/// Recibo completo e auditável da pontuação da partida de um jogador.
/// FinalMps = Clamp(0, 100, BaseScore + (SubtotalRaw − PositionBaseline) × ContextMultiplier.Combined).
/// </summary>
/// <param name="SubtotalRaw">Soma dos TotalPoints de ActionBreakdown e PenaltyBreakdown.</param>
/// <param name="PositionBaseline">
/// Pontuação esperada de uma atuação média na posição, proporcional aos minutos: o que o jogador precisa
/// superar para ficar acima de 50.
/// </param>
/// <param name="CountsTowardsSeason">
/// Falso quando o jogador atuou menos de 20 minutos sem ação decisiva no placar: a partida não entra no FSS.
/// </param>
public sealed record MatchReceipt(
    Guid PlayerId,
    Guid MatchId,
    PlayerPosition PositionEvaluated,
    decimal BaseScore,
    IReadOnlyList<ActionScoreItem> ActionBreakdown,
    IReadOnlyList<ActionScoreItem> PenaltyBreakdown,
    decimal SubtotalRaw,
    decimal PositionBaseline,
    ContextMultiplier ContextMultiplier,
    decimal MinutesFactor,
    decimal FinalMps,
    int AlgorithmVersion,
    DateTime CalculatedAt,
    bool CountsTowardsSeason = true
);
