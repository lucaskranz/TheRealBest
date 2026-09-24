namespace TheRealBest.Domain.Entities;

using TheRealBest.Domain.Enums;

public class MatchPerformanceScore : EntityBase
{
    public Guid MatchId { get; private set; }
    public virtual Match Match { get; private set; } = null!;

    public Guid PlayerId { get; private set; }
    public virtual Player Player { get; private set; } = null!;

    public Guid MatchPlayerStatsId { get; private set; }
    public virtual MatchPlayerStats MatchPlayerStats { get; private set; } = null!;

    public PlayerPosition PositionEvaluated { get; private set; }
    public decimal BaseScore { get; private set; } = 50.0m;
    public string ActionBreakdownJson { get; private set; } = "[]";
    public string PenaltyBreakdownJson { get; private set; } = "[]";
    public decimal SubtotalRaw { get; private set; }
    public decimal TournamentMultiplier { get; private set; } = 1.0m;
    public decimal OpponentMultiplier { get; private set; } = 1.0m;
    public decimal ClutchMultiplier { get; private set; } = 1.0m;
    public decimal ContextMultiplierCombined { get; private set; } = 1.0m;
    public decimal MinutesFactor { get; private set; } = 1.0m;
    public decimal FinalMps { get; private set; }
    public DateTime CalculatedAt { get; private set; }
    public int AlgorithmVersion { get; private set; } = 1;

    protected MatchPerformanceScore() { }

    public MatchPerformanceScore(
        Guid matchId,
        Guid playerId,
        Guid matchPlayerStatsId,
        PlayerPosition positionEvaluated,
        decimal baseScore,
        string actionBreakdownJson,
        string penaltyBreakdownJson,
        decimal subtotalRaw,
        decimal tournamentMultiplier,
        decimal opponentMultiplier,
        decimal clutchMultiplier,
        decimal contextMultiplierCombined,
        decimal minutesFactor,
        decimal finalMps,
        int algorithmVersion = 1,
        DateTime? calculatedAt = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        MatchId = matchId;
        PlayerId = playerId;
        MatchPlayerStatsId = matchPlayerStatsId;
        PositionEvaluated = positionEvaluated;
        BaseScore = baseScore;
        ActionBreakdownJson = actionBreakdownJson;
        PenaltyBreakdownJson = penaltyBreakdownJson;
        SubtotalRaw = subtotalRaw;
        TournamentMultiplier = tournamentMultiplier;
        OpponentMultiplier = opponentMultiplier;
        ClutchMultiplier = clutchMultiplier;
        ContextMultiplierCombined = contextMultiplierCombined;
        MinutesFactor = minutesFactor;
        FinalMps = finalMps;
        AlgorithmVersion = algorithmVersion;
        CalculatedAt = calculatedAt ?? DateTime.UtcNow;
    }
}