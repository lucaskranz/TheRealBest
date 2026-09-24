namespace TheRealBest.Scoring.WeightMatrices;

using TheRealBest.Domain.Enums;

/// <summary>Zagueiro central (CB). Coluna "CB" da especificação do Fair Player Index.</summary>
public static class CenterBackWeights
{
    public static WeightMatrix Matrix { get; } = new(new Dictionary<ActionType, decimal>
    {
        [ActionType.Goal] = 30m,
        [ActionType.PenaltyGoal] = 15m,
        [ActionType.Assist] = 16m,
        [ActionType.ExpectedAssists] = 12m,
        [ActionType.BigChanceCreated] = 8m,
        [ActionType.ShotOnTarget] = 2m,
        [ActionType.ExpectedGoalsOverperformance] = 4m,
        [ActionType.KeyPass] = 2m,
        [ActionType.ProgressivePass] = 1.2m,
        [ActionType.PassAccuracyBonus] = 4m,
        [ActionType.DribbleSuccess] = 2m,
        [ActionType.FoulDrawn] = 1m,
        [ActionType.Tackle] = 4m,
        [ActionType.Interception] = 3.5m,
        [ActionType.AerialDuelWon] = 2.5m,
        [ActionType.DuelWon] = 2m,
        [ActionType.BallRecovery] = 2m,
        [ActionType.CleanSheet] = 14m,

        [ActionType.ErrorLeadingToGoal] = -22m,
        [ActionType.ErrorLeadingToShot] = -8m,
        [ActionType.PenaltyCommitted] = -15m,
        [ActionType.OwnGoal] = -16m,
        [ActionType.YellowCard] = -3m,
        [ActionType.RedCard] = -25m,
        [ActionType.BigChanceMissed] = -2m,
        [ActionType.Turnover] = -6m,
        [ActionType.GoalConceded] = -3m,
    });
}
