namespace TheRealBest.Scoring.Tests.Builders;

using TheRealBest.Domain.Enums;

/// <summary>
/// Linhas estatísticas típicas de um titular numa rodada comum, sem gol nem assistência.
/// Os cenários partem delas e acrescentam apenas o que querem destacar.
/// </summary>
public static class TypicalLines
{
    public static StatsBuilder Goalkeeper() =>
        StatsBuilder.For(PlayerPosition.GK).Saves(3).GoalsConceded(1).Passes(30, 24).AerialDuels(1, 1);

    public static StatsBuilder CenterBack() =>
        StatsBuilder.For(PlayerPosition.CB).Tackles(1).Interceptions(1).AerialDuels(4, 3).Duels(7, 5)
            .Recoveries(5).Passes(60, 54, progressive: 3).GoalsConceded(1);

    public static StatsBuilder FullBack() =>
        StatsBuilder.For(PlayerPosition.FB).Tackles(2).Interceptions(1).AerialDuels(2, 1).Duels(9, 5)
            .Recoveries(5).Passes(50, 42, progressive: 4).KeyPasses(1).Dribbles(1, 1).FoulsDrawn(1).GoalsConceded(1);

    public static StatsBuilder DefensiveMid() =>
        StatsBuilder.For(PlayerPosition.CDM).Tackles(2).Interceptions(1).AerialDuels(2, 1).Duels(10, 5)
            .Recoveries(6).Passes(55, 49, progressive: 5).KeyPasses(1).Dribbles(2, 1).FoulsDrawn(1).GoalsConceded(1);

    public static StatsBuilder Winger() =>
        StatsBuilder.For(PlayerPosition.W).Tackles(1).AerialDuels(1, 0).Duels(12, 5).Recoveries(4)
            .Passes(35, 28, progressive: 3).KeyPasses(2).Dribbles(4, 2).FoulsDrawn(1).Shots(2, 1);

    public static StatsBuilder Striker() =>
        StatsBuilder.For(PlayerPosition.ST).AerialDuels(5, 2).Duels(12, 5).Recoveries(2)
            .Passes(20, 15, progressive: 1).KeyPasses(1).Dribbles(2, 1).FoulsDrawn(1).Shots(3, 1).Offsides(1);
}
