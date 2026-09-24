namespace TheRealBest.Domain.Entities;

using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;

public class MatchPlayerStats : EntityBase
{
    public Guid MatchId { get; private set; }
    public virtual Match Match { get; private set; } = null!;

    public Guid PlayerId { get; private set; }
    public virtual Player Player { get; private set; } = null!;

    public Guid TeamId { get; private set; }
    public virtual Team Team { get; private set; } = null!;

    public PlayerPosition PositionPlayed { get; private set; }
    public int MinutesPlayed { get; private set; }
    public int Goals { get; private set; }
    public int Assists { get; private set; }
    public int ShotsTotal { get; private set; }
    public int ShotsOnTarget { get; private set; }
    public int PassesTotal { get; private set; }
    public int PassesAccurate { get; private set; }
    public decimal PassAccuracy { get; private set; }
    public int KeyPasses { get; private set; }
    public int TacklesTotal { get; private set; }
    public int Interceptions { get; private set; }
    public int Blocks { get; private set; }
    public int DuelsTotal { get; private set; }
    public int DuelsWon { get; private set; }
    public int AerialDuelsWon { get; private set; }
    public int AerialDuelsTotal { get; private set; }
    public int DribblesAttempted { get; private set; }
    public int DribblesSuccess { get; private set; }
    public int DribbledPast { get; private set; }
    public int FoulsCommitted { get; private set; }
    public int FoulsDrawn { get; private set; }
    public int YellowCards { get; private set; }
    public int RedCards { get; private set; }
    public int Offsides { get; private set; }
    public int Saves { get; private set; }
    public int GoalsConceded { get; private set; }
    public int PenaltiesWon { get; private set; }
    public int PenaltiesCommitted { get; private set; }
    public int PenaltiesSaved { get; private set; }
    public int PenaltiesMissed { get; private set; }
    public int PenaltiesScored { get; private set; }
    public bool CleanSheet { get; private set; }
    public decimal Xg { get; private set; }
    public decimal Xa { get; private set; }
    public int BigChancesMissed { get; private set; }
    public int BigChancesCreated { get; private set; }
    public int ErrorsLeadingToGoal { get; private set; }
    public int OwnGoals { get; private set; }
    public int ShotCreatingActions { get; private set; }
    public int ProgressivePasses { get; private set; }
    public int ProgressiveCarries { get; private set; }
    public int BallRecoveries { get; private set; }
    public int Turnovers { get; private set; }
    public int Touches { get; private set; }

    public virtual MatchPerformanceScore? PerformanceScore { get; private set; }

    protected MatchPlayerStats() { }

    /// <summary>
    /// Cria as estatísticas de uma partida a partir de uma linha importada de fonte externa.
    /// </summary>
    public static MatchPlayerStats FromStatLine(Guid matchId, Guid playerId, Guid teamId, PlayerStatLine line)
    {
        var stats = new MatchPlayerStats(matchId, playerId, teamId, line.Position, line.MinutesPlayed);

        stats.SetOffensiveStats(
            line.Goals, line.Assists, line.ShotsTotal, line.ShotsOnTarget, line.KeyPasses,
            line.BigChancesCreated, line.BigChancesMissed, line.Xg, line.Xa, line.ShotCreatingActions,
            line.PenaltiesScored);

        var passAccuracy = line.PassesTotal == 0
            ? 0m
            : Math.Round(line.PassesAccurate * 100m / line.PassesTotal, 2, MidpointRounding.AwayFromZero);
        stats.SetPassingStats(
            line.PassesTotal, line.PassesAccurate, passAccuracy, line.ProgressivePasses,
            line.ProgressiveCarries, line.Touches, line.Turnovers);

        stats.SetDefensiveStats(
            line.TacklesTotal, line.Interceptions, line.Blocks, line.BallRecoveries, line.DuelsTotal, line.DuelsWon,
            line.AerialDuelsTotal, line.AerialDuelsWon, line.DribbledPast, line.ErrorsLeadingToGoal, line.OwnGoals);

        stats.SetGoalkeepingAndDisciplinary(
            line.Saves, line.GoalsConceded, line.PenaltiesSaved, line.CleanSheet, line.FoulsCommitted, line.FoulsDrawn,
            line.YellowCards, line.RedCards, line.PenaltiesWon, line.PenaltiesCommitted, line.PenaltiesMissed,
            line.DribblesAttempted, line.DribblesSuccess, line.Offsides);

        return stats;
    }

    public MatchPlayerStats(
        Guid matchId,
        Guid playerId,
        Guid teamId,
        PlayerPosition positionPlayed,
        int minutesPlayed,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        MatchId = matchId;
        PlayerId = playerId;
        TeamId = teamId;
        PositionPlayed = positionPlayed;
        MinutesPlayed = minutesPlayed;
    }

    public void SetOffensiveStats(
        int goals,
        int assists,
        int shotsTotal,
        int shotsOnTarget,
        int keyPasses,
        int bigChancesCreated,
        int bigChancesMissed,
        decimal xg,
        decimal xa,
        int shotCreatingActions,
        int penaltiesScored = 0)
    {
        Goals = goals;
        Assists = assists;
        ShotsTotal = shotsTotal;
        ShotsOnTarget = shotsOnTarget;
        KeyPasses = keyPasses;
        BigChancesCreated = bigChancesCreated;
        BigChancesMissed = bigChancesMissed;
        Xg = xg;
        Xa = xa;
        ShotCreatingActions = shotCreatingActions;
        PenaltiesScored = penaltiesScored;
        MarkUpdated();
    }

    public void SetPassingStats(
        int passesTotal,
        int passesAccurate,
        decimal passAccuracy,
        int progressivePasses,
        int progressiveCarries,
        int touches,
        int turnovers)
    {
        PassesTotal = passesTotal;
        PassesAccurate = passesAccurate;
        PassAccuracy = passAccuracy;
        ProgressivePasses = progressivePasses;
        ProgressiveCarries = progressiveCarries;
        Touches = touches;
        Turnovers = turnovers;
        MarkUpdated();
    }

    public void SetDefensiveStats(
        int tacklesTotal,
        int interceptions,
        int blocks,
        int ballRecoveries,
        int duelsTotal,
        int duelsWon,
        int aerialDuelsTotal,
        int aerialDuelsWon,
        int dribbledPast,
        int errorsLeadingToGoal,
        int ownGoals = 0)
    {
        TacklesTotal = tacklesTotal;
        Interceptions = interceptions;
        Blocks = blocks;
        BallRecoveries = ballRecoveries;
        DuelsTotal = duelsTotal;
        DuelsWon = duelsWon;
        AerialDuelsTotal = aerialDuelsTotal;
        AerialDuelsWon = aerialDuelsWon;
        DribbledPast = dribbledPast;
        ErrorsLeadingToGoal = errorsLeadingToGoal;
        OwnGoals = ownGoals;
        MarkUpdated();
    }

    public void SetGoalkeepingAndDisciplinary(
        int saves,
        int goalsConceded,
        int penaltiesSaved,
        bool cleanSheet,
        int foulsCommitted,
        int foulsDrawn,
        int yellowCards,
        int redCards,
        int penaltiesWon,
        int penaltiesCommitted,
        int penaltiesMissed,
        int dribblesAttempted,
        int dribblesSuccess,
        int offsides)
    {
        Saves = saves;
        GoalsConceded = goalsConceded;
        PenaltiesSaved = penaltiesSaved;
        CleanSheet = cleanSheet;
        FoulsCommitted = foulsCommitted;
        FoulsDrawn = foulsDrawn;
        YellowCards = yellowCards;
        RedCards = redCards;
        PenaltiesWon = penaltiesWon;
        PenaltiesCommitted = penaltiesCommitted;
        PenaltiesMissed = penaltiesMissed;
        DribblesAttempted = dribblesAttempted;
        DribblesSuccess = dribblesSuccess;
        Offsides = offsides;
        MarkUpdated();
    }
}