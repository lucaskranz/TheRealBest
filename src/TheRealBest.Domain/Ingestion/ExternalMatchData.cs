namespace TheRealBest.Domain.Ingestion;

using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;

// Dados importados de uma fonte externa, identificados apenas por IDs externos.
// A ingestão resolve esses IDs para as entidades do banco (criando-as quando necessário).

/// <param name="LocalizedNames">Nome por locale (pt-BR, en, es), quando conhecido. Vira competition_translations.</param>
public sealed record ExternalCompetition(
    string ExternalId,
    string Name,
    string Country,
    CompetitionTier Tier,
    int SeasonYear,
    IReadOnlyDictionary<string, string>? LocalizedNames = null);

public sealed record ExternalTeam(
    string ExternalId,
    string Name,
    string LogoUrl);

public sealed record ExternalFixture(
    string ExternalId,
    ExternalCompetition Competition,
    ExternalTeam HomeTeam,
    ExternalTeam AwayTeam,
    string RoundPhase,
    bool IsKnockout,
    DateTime KickoffUtc,
    bool IsFinished,
    int? HomeScore,
    int? AwayScore);

public sealed record ExternalPlayer(
    string ExternalId,
    string Name,
    string PhotoUrl);

public sealed record ExternalPlayerPerformance(
    ExternalPlayer Player,
    string TeamExternalId,
    PlayerStatLine Stats);

public sealed record ExternalMatchReport(
    ExternalFixture Fixture,
    IReadOnlyList<ExternalPlayerPerformance> Performances);
