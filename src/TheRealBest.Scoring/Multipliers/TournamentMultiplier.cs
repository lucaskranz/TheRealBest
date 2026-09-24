namespace TheRealBest.Scoring.Multipliers;

using TheRealBest.Domain.Enums;

/// <summary>
/// Peso do torneio (W_torneio), seção 4.1 da especificação.
/// </summary>
public static class TournamentMultiplier
{
    public const decimal WorldCup = 1.40m;
    public const decimal ChampionsLeagueKnockout = 1.35m;
    public const decimal ContinentalNationalTeams = 1.30m;
    public const decimal ChampionsLeagueLeaguePhase = 1.20m;
    public const decimal TopLeague = 1.10m;
    public const decimal DomesticCupLateStage = 1.05m;
    public const decimal Other = 0.95m;

    /// <param name="roundPhase">Fase informada pela fonte de dados (ex.: "Semi-finals", "Final", "3rd Round").</param>
    public static decimal For(CompetitionTier tier, bool isKnockout, string roundPhase) => tier switch
    {
        CompetitionTier.WorldCup => WorldCup,
        CompetitionTier.UclKnockout => isKnockout ? ChampionsLeagueKnockout : ChampionsLeagueLeaguePhase,
        CompetitionTier.InternationalContinental => ContinentalNationalTeams,
        CompetitionTier.TopLeague => TopLeague,
        CompetitionTier.DomesticCup => IsSemiFinalOrFinal(roundPhase) ? DomesticCupLateStage : Other,
        _ => Other,
    };

    private static bool IsSemiFinalOrFinal(string roundPhase)
    {
        var phase = roundPhase.Trim();
        return phase.Contains("semi", StringComparison.OrdinalIgnoreCase)
            || phase.Equals("final", StringComparison.OrdinalIgnoreCase);
    }
}
