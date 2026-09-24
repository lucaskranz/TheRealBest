namespace TheRealBest.Scoring.Calculators;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;

/// <summary>
/// Converte as estatísticas brutas da partida em quantidades por ação pontuável.
/// </summary>
/// <remarks>
/// Ações da especificação sem fonte de dados confiável hoje ficam de fora até a ingestão fornecê-las:
/// GoalsPrevented (xGOT), HighClaim, ErrorLeadingToShot e Turnover (a spec pune só perdas no
/// campo defensivo; a fonte atual informa apenas o total de perdas).
/// </remarks>
public static class StatsActionMapper
{
    /// <summary>Mínimo de passes para o bônus de precisão (evita 3 de 3 = 100%).</summary>
    public const int MinPassesForAccuracyBonus = 20;

    public const decimal PassAccuracyBonusThreshold = 85m;

    /// <summary>Clean sheet só conta para quem jogou mais de 60 minutos.</summary>
    public const int CleanSheetMinMinutes = 60;

    public static IReadOnlyDictionary<ActionType, decimal> Map(MatchPlayerStats stats)
    {
        var openPlayGoals = Math.Max(0, stats.Goals - stats.PenaltiesScored);

        return new Dictionary<ActionType, decimal>
        {
            [ActionType.Goal] = openPlayGoals,
            [ActionType.PenaltyGoal] = stats.PenaltiesScored,
            [ActionType.Assist] = stats.Assists,
            [ActionType.ExpectedAssists] = stats.Xa,
            [ActionType.BigChanceCreated] = stats.BigChancesCreated,
            [ActionType.ShotOnTarget] = stats.ShotsOnTarget,
            // Sem xG na fonte o campo fica 0; como todo gol tem xG > 0, xG = 0 significa "sem dado", não "sem chance"
            [ActionType.ExpectedGoalsOverperformance] = stats.Xg > 0m ? Math.Max(0m, stats.Goals - stats.Xg) : 0m,
            [ActionType.KeyPass] = stats.KeyPasses,
            [ActionType.ProgressivePass] = stats.ProgressivePasses,
            [ActionType.PassAccuracyBonus] = HasPassAccuracyBonus(stats) ? 1 : 0,
            [ActionType.DribbleSuccess] = stats.DribblesSuccess,
            [ActionType.FoulDrawn] = stats.FoulsDrawn,
            [ActionType.Tackle] = stats.TacklesTotal,
            [ActionType.Interception] = stats.Interceptions,
            [ActionType.AerialDuelWon] = stats.AerialDuelsWon,
            // Os duelos totais da fonte incluem os aéreos
            [ActionType.DuelWon] = Math.Max(0, stats.DuelsWon - stats.AerialDuelsWon),
            [ActionType.BallRecovery] = stats.BallRecoveries,
            [ActionType.CleanSheet] = stats.CleanSheet && stats.MinutesPlayed > CleanSheetMinMinutes ? 1 : 0,
            [ActionType.Save] = stats.Saves,
            [ActionType.PenaltySaved] = stats.PenaltiesSaved,

            [ActionType.ErrorLeadingToGoal] = stats.ErrorsLeadingToGoal,
            [ActionType.OwnGoal] = stats.OwnGoals,
            [ActionType.PenaltyCommitted] = stats.PenaltiesCommitted,
            [ActionType.YellowCard] = stats.YellowCards,
            [ActionType.RedCard] = stats.RedCards,
            [ActionType.BigChanceMissed] = stats.BigChancesMissed,
            [ActionType.Offside] = stats.Offsides,
            // A spec pune os gols sofridos a partir do segundo
            [ActionType.GoalConceded] = Math.Max(0, stats.GoalsConceded - 1),
        };
    }

    private static bool HasPassAccuracyBonus(MatchPlayerStats stats) =>
        stats.PassesTotal >= MinPassesForAccuracyBonus
        && stats.PassesAccurate * 100m / stats.PassesTotal > PassAccuracyBonusThreshold;
}
