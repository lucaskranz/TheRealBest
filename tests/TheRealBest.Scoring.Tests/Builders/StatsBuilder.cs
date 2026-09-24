namespace TheRealBest.Scoring.Tests.Builders;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;

/// <summary>
/// Monta a linha estatística de um jogador numa partida. Tudo começa zerado.
/// </summary>
public sealed class StatsBuilder
{
    private readonly PlayerPosition _position;
    private int _minutes = 90;
    private int _goals, _penaltiesScored, _assists, _shotsTotal, _shotsOnTarget, _keyPasses;
    private int _bigChancesCreated, _bigChancesMissed, _shotCreatingActions;
    private decimal _xg, _xa;
    private int _passesTotal, _passesAccurate, _progressivePasses, _progressiveCarries, _touches, _turnovers;
    private int _tackles, _interceptions, _blocks, _recoveries, _duelsTotal, _duelsWon;
    private int _aerialTotal, _aerialWon, _dribbledPast, _errorsLeadingToGoal;
    private int _saves, _goalsConceded, _penaltiesSaved, _foulsCommitted, _foulsDrawn;
    private int _yellow, _red, _penaltiesWon, _penaltiesCommitted, _penaltiesMissed;
    private int _dribblesAttempted, _dribblesSuccess, _offsides;
    private bool _cleanSheet;

    private StatsBuilder(PlayerPosition position) => _position = position;

    public static StatsBuilder For(PlayerPosition position) => new(position);

    public StatsBuilder Minutes(int minutes) { _minutes = minutes; return this; }

    public StatsBuilder Goals(int goals, int penalties = 0) { _goals = goals; _penaltiesScored = penalties; return this; }

    public StatsBuilder Assists(int assists) { _assists = assists; return this; }

    public StatsBuilder Shots(int total, int onTarget) { _shotsTotal = total; _shotsOnTarget = onTarget; return this; }

    public StatsBuilder ExpectedGoals(decimal xg) { _xg = xg; return this; }

    public StatsBuilder ExpectedAssists(decimal xa) { _xa = xa; return this; }

    public StatsBuilder KeyPasses(int keyPasses) { _keyPasses = keyPasses; return this; }

    public StatsBuilder BigChances(int created = 0, int missed = 0) { _bigChancesCreated = created; _bigChancesMissed = missed; return this; }

    public StatsBuilder Passes(int total, int accurate, int progressive = 0)
    {
        _passesTotal = total;
        _passesAccurate = accurate;
        _progressivePasses = progressive;
        return this;
    }

    public StatsBuilder Tackles(int tackles) { _tackles = tackles; return this; }

    public StatsBuilder Interceptions(int interceptions) { _interceptions = interceptions; return this; }

    public StatsBuilder Recoveries(int recoveries) { _recoveries = recoveries; return this; }

    /// <summary>Duelos totais incluem os aéreos, como na fonte de dados.</summary>
    public StatsBuilder Duels(int total, int won) { _duelsTotal = total; _duelsWon = won; return this; }

    public StatsBuilder AerialDuels(int total, int won) { _aerialTotal = total; _aerialWon = won; return this; }

    public StatsBuilder Dribbles(int attempted, int success) { _dribblesAttempted = attempted; _dribblesSuccess = success; return this; }

    public StatsBuilder FoulsDrawn(int foulsDrawn) { _foulsDrawn = foulsDrawn; return this; }

    public StatsBuilder Saves(int saves) { _saves = saves; return this; }

    public StatsBuilder GoalsConceded(int goalsConceded) { _goalsConceded = goalsConceded; return this; }

    public StatsBuilder PenaltiesSaved(int penaltiesSaved) { _penaltiesSaved = penaltiesSaved; return this; }

    public StatsBuilder CleanSheet() { _cleanSheet = true; return this; }

    public StatsBuilder Cards(int yellow = 0, int red = 0) { _yellow = yellow; _red = red; return this; }

    public StatsBuilder PenaltiesCommitted(int penaltiesCommitted) { _penaltiesCommitted = penaltiesCommitted; return this; }

    public StatsBuilder ErrorsLeadingToGoal(int errors) { _errorsLeadingToGoal = errors; return this; }

    public StatsBuilder Offsides(int offsides) { _offsides = offsides; return this; }

    public MatchPlayerStats Build()
    {
        var stats = new MatchPlayerStats(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _position, _minutes);

        stats.SetOffensiveStats(
            _goals, _assists, _shotsTotal, _shotsOnTarget, _keyPasses,
            _bigChancesCreated, _bigChancesMissed, _xg, _xa, _shotCreatingActions, _penaltiesScored);

        var passAccuracy = _passesTotal == 0 ? 0m : Math.Round(_passesAccurate * 100m / _passesTotal, 2);
        stats.SetPassingStats(
            _passesTotal, _passesAccurate, passAccuracy, _progressivePasses, _progressiveCarries, _touches, _turnovers);

        stats.SetDefensiveStats(
            _tackles, _interceptions, _blocks, _recoveries, _duelsTotal, _duelsWon,
            _aerialTotal, _aerialWon, _dribbledPast, _errorsLeadingToGoal);

        stats.SetGoalkeepingAndDisciplinary(
            _saves, _goalsConceded, _penaltiesSaved, _cleanSheet, _foulsCommitted, _foulsDrawn,
            _yellow, _red, _penaltiesWon, _penaltiesCommitted, _penaltiesMissed,
            _dribblesAttempted, _dribblesSuccess, _offsides);

        return stats;
    }
}
