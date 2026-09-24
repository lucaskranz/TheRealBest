namespace TheRealBest.Scoring.WeightMatrices;

using TheRealBest.Domain.Enums;

/// <summary>
/// Volante e meio-campista central (CDM e CM). Coluna "Volante/Meia (CM)" da especificação do Fair Player Index.
/// </summary>
public static class DefensiveMidWeights
{
    public static WeightMatrix Matrix { get; } = new(new Dictionary<ActionType, decimal>
    {
        [ActionType.Goal] = 20m,
        [ActionType.PenaltyGoal] = 12m,
        [ActionType.Assist] = 12m,
        [ActionType.ExpectedAssists] = 10m,
        [ActionType.BigChanceCreated] = 6m,
        [ActionType.ShotOnTarget] = 2.5m,
        [ActionType.ExpectedGoalsOverperformance] = 5m,
        [ActionType.KeyPass] = 3m,
        [ActionType.ProgressivePass] = 0.8m,
        [ActionType.PassAccuracyBonus] = 5m,
        [ActionType.DribbleSuccess] = 2.5m,
        [ActionType.FoulDrawn] = 1.5m,
        [ActionType.Tackle] = 3m,
        [ActionType.Interception] = 2.5m,
        [ActionType.AerialDuelWon] = 1.8m,
        [ActionType.DuelWon] = 2m,
        [ActionType.BallRecovery] = 2.2m,
        [ActionType.CleanSheet] = 4m,

        [ActionType.ErrorLeadingToGoal] = -18m,
        [ActionType.ErrorLeadingToShot] = -6m,
        [ActionType.PenaltyCommitted] = -12m,
        [ActionType.OwnGoal] = -12m,
        [ActionType.YellowCard] = -3m,
        [ActionType.RedCard] = -25m,
        [ActionType.BigChanceMissed] = -4m,
        [ActionType.Turnover] = -4m,
        [ActionType.Offside] = -1m,
        [ActionType.GoalConceded] = -1m,
    });
}
