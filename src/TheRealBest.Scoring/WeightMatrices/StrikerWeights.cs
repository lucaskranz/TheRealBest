namespace TheRealBest.Scoring.WeightMatrices;

using TheRealBest.Domain.Enums;

/// <summary>Centroavante (ST). Coluna "ST" da especificação do Fair Player Index.</summary>
public static class StrikerWeights
{
    public static WeightMatrix Matrix { get; } = new(new Dictionary<ActionType, decimal>
    {
        [ActionType.Goal] = 12m,
        [ActionType.PenaltyGoal] = 8m,
        [ActionType.Assist] = 8m,
        [ActionType.ExpectedAssists] = 6m,
        [ActionType.BigChanceCreated] = 4m,
        [ActionType.ShotOnTarget] = 3m,
        [ActionType.ExpectedGoalsOverperformance] = 7m,
        [ActionType.KeyPass] = 2m,
        [ActionType.ProgressivePass] = 0.4m,
        [ActionType.PassAccuracyBonus] = 2m,
        [ActionType.DribbleSuccess] = 2.5m,
        [ActionType.FoulDrawn] = 2m,
        [ActionType.Tackle] = 1.5m,
        [ActionType.Interception] = 1m,
        [ActionType.AerialDuelWon] = 1.8m,
        [ActionType.DuelWon] = 1.5m,
        [ActionType.BallRecovery] = 1m,

        [ActionType.ErrorLeadingToGoal] = -10m,
        [ActionType.ErrorLeadingToShot] = -3m,
        [ActionType.PenaltyCommitted] = -8m,
        [ActionType.OwnGoal] = -10m,
        [ActionType.YellowCard] = -3m,
        [ActionType.RedCard] = -25m,
        [ActionType.BigChanceMissed] = -8m,
        [ActionType.Turnover] = -1m,
        [ActionType.Offside] = -2m,
    });
}
