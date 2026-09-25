namespace TheRealBest.Infrastructure.Ingestion;

using System.Globalization;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <summary>
/// Partidas de uma competição que fazem parte da temporada: encerradas, da temporada pedida (torneios de seleções) e, nas
/// copas nacionais, com ao menos um time da primeira divisão do país. Compartilhado pela importação e pela conferência,
/// para as duas contarem as mesmas partidas.
/// </summary>
public sealed class SeasonFixtureSelector(IFootballDataProvider dataProvider)
{
    private readonly Dictionary<(int League, int ApiSeason), IReadOnlySet<string>> _teamsByLeague = [];

    public async Task<IReadOnlyList<ExternalFixture>> SelectAsync(
        SeasonScopeEntry entry,
        int season,
        IReadOnlyList<ExternalFixture> listed,
        CancellationToken cancellationToken)
    {
        var finished = listed.Where(f => f.IsFinished && f.Competition.SeasonYear == season).ToList();
        if (ApiFootballSeasonScope.TopLeagueOf(entry.LeagueId) is not { } topLeague)
        {
            return finished;
        }

        var topLeagueTeams = await TeamsOfAsync(topLeague, entry.ApiSeason, cancellationToken);
        return finished
            .Where(f => topLeagueTeams.Contains(f.HomeTeam.ExternalId) || topLeagueTeams.Contains(f.AwayTeam.ExternalId))
            .ToList();
    }

    /// <summary>Custa 1 requisição por liga e temporada, zero quando a listagem está no cache (temporadas encerradas).</summary>
    private async Task<IReadOnlySet<string>> TeamsOfAsync(int league, int apiSeason, CancellationToken cancellationToken)
    {
        if (!_teamsByLeague.TryGetValue((league, apiSeason), out var teams))
        {
            var fixtures = await dataProvider.GetFixturesAsync(league.ToString(CultureInfo.InvariantCulture), apiSeason, cancellationToken);
            teams = fixtures.SelectMany(f => new[] { f.HomeTeam.ExternalId, f.AwayTeam.ExternalId }).ToHashSet();
            _teamsByLeague[(league, apiSeason)] = teams;
        }

        return teams;
    }
}
