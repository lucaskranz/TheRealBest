namespace TheRealBest.Domain.Entities;

using TheRealBest.Domain.Enums;

public class Player : EntityBase
{
    public string ExternalApiId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Nationality { get; private set; } = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }
    public string PhotoUrl { get; private set; } = string.Empty;
    public PlayerPosition PrimaryPosition { get; private set; }

    // Navigation properties
    private readonly List<MatchPlayerStats> _matchStats = [];
    public virtual IReadOnlyCollection<MatchPlayerStats> MatchStats => _matchStats.AsReadOnly();

    private readonly List<MatchPerformanceScore> _performanceScores = [];
    public virtual IReadOnlyCollection<MatchPerformanceScore> PerformanceScores => _performanceScores.AsReadOnly();

    private readonly List<SeasonRanking> _seasonRankings = [];
    public virtual IReadOnlyCollection<SeasonRanking> SeasonRankings => _seasonRankings.AsReadOnly();

    protected Player() { }

    public Player(
        string externalApiId,
        string name,
        string nationality,
        DateOnly? dateOfBirth,
        string photoUrl,
        PlayerPosition primaryPosition,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        ExternalApiId = externalApiId;
        Name = name;
        Nationality = nationality;
        DateOfBirth = dateOfBirth;
        PhotoUrl = photoUrl;
        PrimaryPosition = primaryPosition;
    }

    public void UpdateProfile(string name, string nationality, string photoUrl, PlayerPosition primaryPosition)
    {
        Name = name;
        Nationality = nationality;
        PhotoUrl = photoUrl;
        PrimaryPosition = primaryPosition;
        MarkUpdated();
    }
}