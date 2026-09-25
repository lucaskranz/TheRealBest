namespace TheRealBest.Infrastructure.Ingestion;

using System.Globalization;
using Microsoft.Extensions.Logging;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <param name="Expected">Partidas encerradas da competição na temporada, segundo a API (mesma seleção da importação).</param>
/// <param name="Stored">Dessas, quantas estão no banco.</param>
/// <param name="WithoutPlayerData">Gravadas sem estatísticas de jogador (cobertura limitada da fonte).</param>
/// <param name="WithoutTacticalPositions">Com jogadores, mas todos na posição padrão da letra da API (escalação sem grid).</param>
/// <param name="MissingElo">Partidas de clubes sem o Elo de ao menos um dos times.</param>
/// <param name="Available">Falso quando a listagem não pôde ser obtida (cota esgotada ou temporada fora do plano).</param>
public sealed record CompetitionAuditLine(
    int LeagueId,
    int ApiSeason,
    string Name,
    int Expected,
    int Stored,
    int WithoutPlayerData,
    int WithoutTacticalPositions,
    int MissingElo,
    bool Available = true)
{
    public int Missing => Expected - Stored;
}

/// <param name="TeamsMissingElo">Clubes sem rating e em quantas partidas: candidatos a apelido em ClubEloNames.</param>
public sealed record SeasonAuditReport(
    int Season,
    IReadOnlyList<CompetitionAuditLine> Competitions,
    IReadOnlyList<(string Team, int Matches)> TeamsMissingElo)
{
    public bool IsComplete => Competitions.All(c => c.Available && c.Missing == 0);
}

public interface ISeasonAuditor
{
    Task<SeasonAuditReport> AuditAsync(int season, int? competitionId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Relatório de conferência de uma temporada importada: partidas esperadas × gravadas por competição, partidas sem
/// estatísticas de jogador, sem posição tática e sem Elo. Custa 1 requisição por competição, zero quando a listagem
/// está no cache (temporadas encerradas).
/// </summary>
public sealed class SeasonAuditor(
    IFootballDataProvider dataProvider,
    IMatchRepository matchRepository,
    ILogger<SeasonAuditor> logger) : ISeasonAuditor
{
    public async Task<SeasonAuditReport> AuditAsync(int season, int? competitionId = null, CancellationToken cancellationToken = default)
    {
        var scope = ApiFootballSeasonScope.For(season)
            .Where(e => competitionId is null || e.LeagueId == competitionId)
            .ToList();

        var selector = new SeasonFixtureSelector(dataProvider);
        var lines = new List<CompetitionAuditLine>();
        var missingEloByTeam = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in scope)
        {
            IReadOnlyList<Domain.Ingestion.ExternalFixture> listed;
            List<string> finished;
            try
            {
                listed = await dataProvider.GetFixturesAsync(entry.LeagueId.ToString(CultureInfo.InvariantCulture), entry.ApiSeason, cancellationToken);
                finished = (await selector.SelectAsync(entry, season, listed, cancellationToken)).Select(f => f.ExternalId).ToList();
            }
            catch (ApiFootballException ex) when (ex.Kind is ApiFootballErrorKind.DailyQuotaExceeded
                                                      or ApiFootballErrorKind.RateLimited
                                                      or ApiFootballErrorKind.SeasonNotAvailable)
            {
                logger.LogWarning("Listagem da liga {League} indisponível: {Message}", entry.LeagueId, ex.Message);
                lines.Add(new CompetitionAuditLine(entry.LeagueId, entry.ApiSeason, $"league {entry.LeagueId}", 0, 0, 0, 0, 0, Available: false));
                continue;
            }

            var stored = await matchRepository.GetImportAuditAsync(finished, cancellationToken);

            var missingElo = stored.Where(m => m.IsClubMatch && (m.HomeEloRating is null || m.AwayEloRating is null)).ToList();
            foreach (var match in missingElo)
            {
                if (match.HomeEloRating is null) Count(missingEloByTeam, match.HomeTeam);
                if (match.AwayEloRating is null) Count(missingEloByTeam, match.AwayTeam);
            }

            lines.Add(new CompetitionAuditLine(
                entry.LeagueId,
                entry.ApiSeason,
                listed.FirstOrDefault()?.Competition.Name ?? $"league {entry.LeagueId}",
                Expected: finished.Count,
                Stored: stored.Count,
                WithoutPlayerData: stored.Count(m => m.PlayerCount == 0),
                WithoutTacticalPositions: stored.Count(m => m.PlayerCount > 0 && m.TacticalPositionCount == 0),
                MissingElo: missingElo.Count));
        }

        var report = new SeasonAuditReport(
            season,
            lines,
            missingEloByTeam.OrderByDescending(t => t.Value).ThenBy(t => t.Key).Select(t => (t.Key, t.Value)).ToList());
        Log(report);
        return report;
    }

    private static void Count(Dictionary<string, int> counts, string team) => counts[team] = counts.GetValueOrDefault(team) + 1;

    private void Log(SeasonAuditReport report)
    {
        logger.LogInformation("Conferência da temporada {Season}/{Next:00}:", report.Season, (report.Season + 1) % 100);
        foreach (var line in report.Competitions)
        {
            if (!line.Available)
            {
                logger.LogWarning("  {Competition,-28} listagem indisponível", line.Name);
                continue;
            }

            logger.LogInformation(
                "  {Competition,-28} esperadas {Expected,4} | gravadas {Stored,4} | faltando {Missing,4} | sem dados de jogador {Empty,3} | sem posição tática {NoGrid,3} | sem Elo {NoElo,4}",
                line.Name, line.Expected, line.Stored, line.Missing, line.WithoutPlayerData, line.WithoutTacticalPositions, line.MissingElo);
        }

        logger.LogInformation(
            "  {Total,-28} esperadas {Expected,4} | gravadas {Stored,4} | faltando {Missing,4} | sem dados de jogador {Empty,3} | sem posição tática {NoGrid,3} | sem Elo {NoElo,4}",
            "TOTAL",
            report.Competitions.Sum(c => c.Expected),
            report.Competitions.Sum(c => c.Stored),
            report.Competitions.Sum(c => c.Missing),
            report.Competitions.Sum(c => c.WithoutPlayerData),
            report.Competitions.Sum(c => c.WithoutTacticalPositions),
            report.Competitions.Sum(c => c.MissingElo));

        if (report.TeamsMissingElo.Count > 0)
        {
            logger.LogInformation("Clubes sem Elo ({Count}): {Teams}", report.TeamsMissingElo.Count,
                string.Join(", ", report.TeamsMissingElo.Select(t => $"{t.Team} ({t.Matches})")));
        }
    }
}
