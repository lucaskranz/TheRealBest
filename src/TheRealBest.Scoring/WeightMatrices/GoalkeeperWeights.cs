namespace TheRealBest.Scoring.WeightMatrices;

using TheRealBest.Domain.Enums;

/// <summary>Goleiro (GK). Coluna "GK" da especificação do Fair Player Index.</summary>
public static class GoalkeeperWeights
{
    public static WeightMatrix Matrix { get; } = new(new Dictionary<ActionType, decimal>
    {
        [ActionType.Goal] = 50m,
        [ActionType.PenaltyGoal] = 30m,
        [ActionType.Assist] = 25m,
        [ActionType.ExpectedAssists] = 15m,
        [ActionType.BigChanceCreated] = 10m,
        [ActionType.KeyPass] = 4m,
        [ActionType.ProgressivePass] = 1.5m,
        [ActionType.PassAccuracyBonus] = 4m,
        [ActionType.AerialDuelWon] = 1m,
        [ActionType.BallRecovery] = 1m,
        [ActionType.CleanSheet] = 18m,
        [ActionType.Save] = 6m,
        [ActionType.GoalsPrevented] = 12m,
        [ActionType.PenaltySaved] = 25m,
        [ActionType.HighClaim] = 4m,

        [ActionType.ErrorLeadingToGoal] = -25m,
        [ActionType.ErrorLeadingToShot] = -10m,
        [ActionType.PenaltyCommitted] = -18m,
        [ActionType.OwnGoal] = -18m,
        [ActionType.YellowCard] = -3m,
        [ActionType.RedCard] = -25m,
        [ActionType.Turnover] = -8m,
        [ActionType.GoalConceded] = -4m,
    });
}
