namespace TheRealBest.Scoring.WeightMatrices;

using TheRealBest.Domain.Enums;

/// <summary>Lateral / ala (FB). Coluna "FB" da especificação do Fair Player Index.</summary>
public static class FullBackWeights
{
    public static WeightMatrix Matrix { get; } = new(new Dictionary<ActionType, decimal>
    {
        [ActionType.Goal] = 25m,
        [ActionType.PenaltyGoal] = 14m,
        [ActionType.Assist] = 14m,
        [ActionType.ExpectedAssists] = 10m,
        [ActionType.BigChanceCreated] = 7m,
        [ActionType.ShotOnTarget] = 2m,
        [ActionType.ExpectedGoalsOverperformance] = 4m,
        [ActionType.KeyPass] = 2.5m,
        [ActionType.ProgressivePass] = 1.0m,
        [ActionType.PassAccuracyBonus] = 3m,
        [ActionType.DribbleSuccess] = 2.5m,
        [ActionType.FoulDrawn] = 1.5m,
        [ActionType.Tackle] = 3.5m,
        [ActionType.Interception] = 3m,
        [ActionType.AerialDuelWon] = 1.8m,
        [ActionType.DuelWon] = 2m,
        [ActionType.BallRecovery] = 2m,
        [ActionType.CleanSheet] = 10m,

        [ActionType.ErrorLeadingToGoal] = -20m,
        [ActionType.ErrorLeadingToShot] = -8m,
        [ActionType.PenaltyCommitted] = -15m,
        [ActionType.OwnGoal] = -15m,
        [ActionType.YellowCard] = -3m,
        [ActionType.RedCard] = -25m,
        [ActionType.BigChanceMissed] = -3m,
        [ActionType.Turnover] = -5m,
        [ActionType.Offside] = -1m,
        [ActionType.GoalConceded] = -2.5m,
    });
}
