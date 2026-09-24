namespace TheRealBest.Scoring.Tests.Builders;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// Partidas reais usadas como cenários. Placar, fase, autor dos gols e minutos em campo são factuais;
/// as demais estatísticas são APROXIMADAS, para fins de teste, e não dados oficiais da partida.
/// </summary>
public static class RealMatches
{
    /// <summary>Final da UCL 2023: Manchester City 1 × 0 Inter. Rodri marcou o gol do título aos 68'.</summary>
    public static (MatchPlayerStats Stats, MatchContext Context) RodriUclFinal2023() =>
    (
        StatsBuilder.For(PlayerPosition.CDM)
            .Goals(1).Shots(2, 1).ExpectedGoals(0.10m).KeyPasses(1)
            .Passes(70, 64, progressive: 8).Dribbles(1, 1).FoulsDrawn(1)
            .Tackles(3).Interceptions(1).AerialDuels(3, 2).Duels(12, 8).Recoveries(7)
            .CleanSheet()
            .Build(),
        Contexts.ChampionsLeagueKnockout(1, 0, opponentElo: 1860)
    );

    /// <summary>Final da UCL 2024: Real Madrid 2 × 0 Borussia Dortmund. Vinícius Jr marcou o 2º gol aos 83'.</summary>
    public static (MatchPlayerStats Stats, MatchContext Context) ViniciusUclFinal2024() =>
    (
        StatsBuilder.For(PlayerPosition.W)
            .Goals(1).Shots(4, 2).ExpectedGoals(0.40m).ExpectedAssists(0.30m).KeyPasses(2).BigChances(created: 1)
            .Passes(35, 28, progressive: 2).Dribbles(7, 3).FoulsDrawn(3)
            .AerialDuels(1, 0).Duels(18, 8).Recoveries(3)
            .Build(),
        Contexts.ChampionsLeagueKnockout(2, 0, opponentElo: 1840)
    );

    /// <summary>
    /// Oitavas da UCL 2023 (volta): Manchester City 7 × 0 RB Leipzig. Haaland marcou 5 gols (1 de pênalti) e saiu aos 63'.
    /// </summary>
    public static (MatchPlayerStats Stats, MatchContext Context) HaalandFiveGoalsVsLeipzig2023() =>
    (
        StatsBuilder.For(PlayerPosition.ST)
            .Minutes(63).Goals(5, penalties: 1).Shots(7, 6).ExpectedGoals(3.20m)
            .Passes(12, 9).AerialDuels(3, 1).Duels(7, 4).Recoveries(1).FoulsDrawn(1)
            .Build(),
        Contexts.ChampionsLeagueKnockout(7, 0, opponentElo: 1830, phase: "Round of 16")
    );

    /// <summary>
    /// Quartas da UCL 2024 (volta): Manchester City 1 × 1 Real Madrid, 120 minutos (Real classificado nos pênaltis).
    /// Rüdiger jogou a prorrogação inteira; a disputa de pênaltis não entra na pontuação.
    /// </summary>
    public static (MatchPlayerStats Stats, MatchContext Context) RudigerVsManCity2024() =>
    (
        StatsBuilder.For(PlayerPosition.CB)
            .Minutes(120).Tackles(3).Interceptions(2).AerialDuels(7, 5).Duels(14, 9).Recoveries(8)
            .Passes(50, 42, progressive: 2).GoalsConceded(1)
            .Build(),
        Contexts.ChampionsLeagueKnockout(1, 1, opponentElo: 2050, phase: "Quarter-finals")
    );
}
