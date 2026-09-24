namespace TheRealBest.Domain.Entities;

public class Team : EntityBase
{
    public string ExternalApiId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ShortName { get; private set; } = string.Empty;
    public string LogoUrl { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public int EloRanking { get; private set; } = 1500;

    // Navigation properties
    private readonly List<Match> _homeMatches = [];
    public virtual IReadOnlyCollection<Match> HomeMatches => _homeMatches.AsReadOnly();

    private readonly List<Match> _awayMatches = [];
    public virtual IReadOnlyCollection<Match> AwayMatches => _awayMatches.AsReadOnly();

    private readonly List<MatchPlayerStats> _playerStats = [];
    public virtual IReadOnlyCollection<MatchPlayerStats> PlayerStats => _playerStats.AsReadOnly();

    protected Team() { }

    public Team(
        string externalApiId,
        string name,
        string shortName,
        string logoUrl,
        string country,
        int eloRanking = 1500,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        ExternalApiId = externalApiId;
        Name = name;
        ShortName = shortName;
        LogoUrl = logoUrl;
        Country = country;
        EloRanking = eloRanking;
    }

    public void UpdateElo(int newElo)
    {
        EloRanking = newElo;
        MarkUpdated();
    }

    public void UpdateDetails(string name, string shortName, string logoUrl, string country)
    {
        Name = name;
        ShortName = shortName;
        LogoUrl = logoUrl;
        Country = country;
        MarkUpdated();
    }
}