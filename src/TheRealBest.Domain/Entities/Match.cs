namespace TheRealBest.Domain.Entities;

public class Match : EntityBase
{
    public string ExternalApiId { get; private set; } = string.Empty;
    public Guid CompetitionId { get; private set; }
    public virtual Competition Competition { get; private set; } = null!;

    public Guid HomeTeamId { get; private set; }
    public virtual Team HomeTeam { get; private set; } = null!;

    public Guid AwayTeamId { get; private set; }
    public virtual Team AwayTeam { get; private set; } = null!;

    public int? HomeScore { get; private set; }
    public int? AwayScore { get; private set; }
    public string RoundPhase { get; private set; } = string.Empty;
    public DateTime MatchDate { get; private set; }
    public bool IsKnockout { get; private set; }

    /// <summary>Rating Elo (escala ClubElo) de cada time na data da partida. Nulo quando indisponível (ex.: seleções).</summary>
    public int? HomeEloRating { get; private set; }
    public int? AwayEloRating { get; private set; }

    // Navigation properties
    private readonly List<MatchPlayerStats> _playerStats = [];
    public virtual IReadOnlyCollection<MatchPlayerStats> PlayerStats => _playerStats.AsReadOnly();

    private readonly List<MatchPerformanceScore> _performanceScores = [];
    public virtual IReadOnlyCollection<MatchPerformanceScore> PerformanceScores => _performanceScores.AsReadOnly();

    protected Match() { }

    public Match(
        string externalApiId,
        Guid competitionId,
        Guid homeTeamId,
        Guid awayTeamId,
        string roundPhase,
        DateTime matchDate,
        bool isKnockout,
        int? homeScore = null,
        int? awayScore = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        ExternalApiId = externalApiId;
        CompetitionId = competitionId;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        RoundPhase = roundPhase;
        MatchDate = matchDate;
        IsKnockout = isKnockout;
        HomeScore = homeScore;
        AwayScore = awayScore;
    }

    public void SetEloRatings(int? homeEloRating, int? awayEloRating)
    {
        HomeEloRating = homeEloRating;
        AwayEloRating = awayEloRating;
        MarkUpdated();
    }

    public void SetScore(int homeScore, int awayScore)
    {
        HomeScore = homeScore;
        AwayScore = awayScore;
        MarkUpdated();
    }
}