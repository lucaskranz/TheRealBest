namespace TheRealBest.Domain.Entities;

public class SeasonRanking : EntityBase
{
    public Guid PlayerId { get; private set; }
    public virtual Player Player { get; private set; } = null!;

    public int SeasonYear { get; private set; }
    public int TotalMatches { get; private set; }
    public int TotalMinutes { get; private set; }
    public decimal MpsAverage { get; private set; }
    public decimal MpsSumWeighted { get; private set; }
    public decimal PresenceFactor { get; private set; }
    public decimal FssScore { get; private set; }
    public int OverallRank { get; private set; }
    public int PositionRank { get; private set; }
    public decimal ClutchIndex { get; private set; }
    public string Top5MatchesJson { get; private set; } = "[]";
    public DateTime RecalculatedAt { get; private set; }

    protected SeasonRanking() { }

    public SeasonRanking(
        Guid playerId,
        int seasonYear,
        int totalMatches,
        int totalMinutes,
        decimal mpsAverage,
        decimal mpsSumWeighted,
        decimal presenceFactor,
        decimal fssScore,
        int overallRank,
        int positionRank,
        decimal clutchIndex,
        string top5MatchesJson = "[]",
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        PlayerId = playerId;
        SeasonYear = seasonYear;
        TotalMatches = totalMatches;
        TotalMinutes = totalMinutes;
        MpsAverage = mpsAverage;
        MpsSumWeighted = mpsSumWeighted;
        PresenceFactor = presenceFactor;
        FssScore = fssScore;
        OverallRank = overallRank;
        PositionRank = positionRank;
        ClutchIndex = clutchIndex;
        Top5MatchesJson = top5MatchesJson;
        RecalculatedAt = DateTime.UtcNow;
    }

    public void UpdateRanks(int overallRank, int positionRank)
    {
        OverallRank = overallRank;
        PositionRank = positionRank;
        RecalculatedAt = DateTime.UtcNow;
        MarkUpdated();
    }
}