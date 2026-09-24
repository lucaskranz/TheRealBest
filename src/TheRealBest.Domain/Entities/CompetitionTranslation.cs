namespace TheRealBest.Domain.Entities;

/// <summary>
/// Tradução do nome de uma competição para um locale suportado (pt-BR, en, es).
/// </summary>
public class CompetitionTranslation : EntityBase
{
    public Guid CompetitionId { get; private set; }
    public virtual Competition Competition { get; private set; } = null!;

    public string Locale { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? CountryName { get; private set; }

    protected CompetitionTranslation() { }

    public CompetitionTranslation(
        Guid competitionId,
        string locale,
        string name,
        string? countryName = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        CompetitionId = competitionId;
        Locale = locale;
        Name = name;
        CountryName = countryName;
    }

    public void Update(string name, string? countryName)
    {
        Name = name;
        CountryName = countryName;
        MarkUpdated();
    }
}
