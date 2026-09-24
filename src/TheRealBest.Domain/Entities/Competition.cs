namespace TheRealBest.Domain.Entities;

using TheRealBest.Domain.Enums;

public class Competition : EntityBase
{
    public string ExternalApiId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public CompetitionTier Tier { get; private set; }
    public decimal TournamentMultiplier { get; private set; } = 1.0m;
    public int SeasonYear { get; private set; }

    // Navigation properties
    private readonly List<Match> _matches = [];
    public virtual IReadOnlyCollection<Match> Matches => _matches.AsReadOnly();

    private readonly List<CompetitionTranslation> _translations = [];
    public virtual IReadOnlyCollection<CompetitionTranslation> Translations => _translations.AsReadOnly();

    protected Competition() { }

    public Competition(
        string externalApiId,
        string name,
        string country,
        CompetitionTier tier,
        decimal tournamentMultiplier,
        int seasonYear,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        ExternalApiId = externalApiId;
        Name = name;
        Country = country;
        Tier = tier;
        TournamentMultiplier = tournamentMultiplier;
        SeasonYear = seasonYear;
    }

    public void UpdateMultiplier(decimal newMultiplier)
    {
        TournamentMultiplier = newMultiplier;
        MarkUpdated();
    }
}