namespace TheRealBest.Scoring.Tests.Builders;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// Monta a linha estatística de um jogador numa partida. Tudo começa zerado; 90 minutos por padrão.
/// </summary>
public sealed class StatsBuilder
{
    private PlayerStatLine _line;

    private StatsBuilder(PlayerPosition position) => _line = new PlayerStatLine { Position = position, MinutesPlayed = 90 };

    public static StatsBuilder For(PlayerPosition position) => new(position);

    public StatsBuilder Minutes(int minutes) => With(_line with { MinutesPlayed = minutes });

    public StatsBuilder Goals(int goals, int penalties = 0) => With(_line with { Goals = goals, PenaltiesScored = penalties });

    public StatsBuilder Assists(int assists) => With(_line with { Assists = assists });

    public StatsBuilder Shots(int total, int onTarget) => With(_line with { ShotsTotal = total, ShotsOnTarget = onTarget });

    public StatsBuilder ExpectedGoals(decimal xg) => With(_line with { Xg = xg });

    public StatsBuilder ExpectedAssists(decimal xa) => With(_line with { Xa = xa });

    public StatsBuilder KeyPasses(int keyPasses) => With(_line with { KeyPasses = keyPasses });

    public StatsBuilder BigChances(int created = 0, int missed = 0) =>
        With(_line with { BigChancesCreated = created, BigChancesMissed = missed });

    public StatsBuilder Passes(int total, int accurate, int progressive = 0) =>
        With(_line with { PassesTotal = total, PassesAccurate = accurate, ProgressivePasses = progressive });

    public StatsBuilder Tackles(int tackles) => With(_line with { TacklesTotal = tackles });

    public StatsBuilder Interceptions(int interceptions) => With(_line with { Interceptions = interceptions });

    public StatsBuilder Recoveries(int recoveries) => With(_line with { BallRecoveries = recoveries });

    /// <summary>Duelos totais incluem os aéreos, como na fonte de dados.</summary>
    public StatsBuilder Duels(int total, int won) => With(_line with { DuelsTotal = total, DuelsWon = won });

    public StatsBuilder AerialDuels(int total, int won) => With(_line with { AerialDuelsTotal = total, AerialDuelsWon = won });

    public StatsBuilder Dribbles(int attempted, int success) =>
        With(_line with { DribblesAttempted = attempted, DribblesSuccess = success });

    public StatsBuilder FoulsDrawn(int foulsDrawn) => With(_line with { FoulsDrawn = foulsDrawn });

    public StatsBuilder Saves(int saves) => With(_line with { Saves = saves });

    public StatsBuilder GoalsConceded(int goalsConceded) => With(_line with { GoalsConceded = goalsConceded });

    public StatsBuilder PenaltiesSaved(int penaltiesSaved) => With(_line with { PenaltiesSaved = penaltiesSaved });

    public StatsBuilder CleanSheet() => With(_line with { CleanSheet = true });

    public StatsBuilder Cards(int yellow = 0, int red = 0) => With(_line with { YellowCards = yellow, RedCards = red });

    public StatsBuilder PenaltiesCommitted(int penaltiesCommitted) => With(_line with { PenaltiesCommitted = penaltiesCommitted });

    public StatsBuilder ErrorsLeadingToGoal(int errors) => With(_line with { ErrorsLeadingToGoal = errors });

    public StatsBuilder OwnGoals(int ownGoals) => With(_line with { OwnGoals = ownGoals });

    public StatsBuilder Offsides(int offsides) => With(_line with { Offsides = offsides });

    public MatchPlayerStats Build() => MatchPlayerStats.FromStatLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _line);

    private StatsBuilder With(PlayerStatLine line)
    {
        _line = line;
        return this;
    }
}
