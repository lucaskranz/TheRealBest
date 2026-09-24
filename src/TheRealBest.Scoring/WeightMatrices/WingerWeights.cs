namespace TheRealBest.Scoring.WeightMatrices;

using TheRealBest.Domain.Enums;

/// <summary>
/// Ponta (W). Coluna "Meia-Atac/Ponta (CAM/W)" da especificação do Fair Player Index: hoje idêntica à do
/// meia-atacante, mantida em classe própria para poder divergir em versões futuras do algoritmo.
/// </summary>
public static class WingerWeights
{
    public static WeightMatrix Matrix { get; } = new(new Dictionary<ActionType, decimal>
    {
        [ActionType.Goal] = 16m,
        [ActionType.PenaltyGoal] = 10m,
        [ActionType.Assist] = 10m,
        [ActionType.ExpectedAssists] = 8m,
        [ActionType.BigChanceCreated] = 5m,
        [ActionType.ShotOnTarget] = 2.5m,
        [ActionType.ExpectedGoalsOverperformance] = 6m,
        [ActionType.KeyPass] = 2.5m,
        [ActionType.ProgressivePass] = 0.6m,
        [ActionType.PassAccuracyBonus] = 3m,
        [ActionType.DribbleSuccess] = 3m,
        [ActionType.FoulDrawn] = 2m,
        [ActionType.Tackle] = 2m,
        [ActionType.Interception] = 1.5m,
        [ActionType.AerialDuelWon] = 1.2m,
        [ActionType.DuelWon] = 1.5m,
        [ActionType.BallRecovery] = 1.5m,

        [ActionType.ErrorLeadingToGoal] = -12m,
        [ActionType.ErrorLeadingToShot] = -4m,
        [ActionType.PenaltyCommitted] = -10m,
        [ActionType.OwnGoal] = -10m,
        [ActionType.YellowCard] = -3m,
        [ActionType.RedCard] = -25m,
        [ActionType.BigChanceMissed] = -6m,
        [ActionType.Turnover] = -2m,
        [ActionType.Offside] = -1.5m,
    });
}
