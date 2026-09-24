namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Globalization;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;

/// <summary>
/// Implementação de <see cref="IFootballDataProvider"/> sobre a API-Football.
/// Custo em requisições: 1 por listagem de partidas; 3 por relatório de partida (jogadores, eventos, escalações).
/// </summary>
public sealed class ApiFootballDataProvider(IApiFootballApi api) : IFootballDataProvider
{
    public async Task<IReadOnlyList<ExternalFixture>> GetFixturesAsync(
        string competitionExternalId,
        int seasonYear,
        CancellationToken cancellationToken = default)
    {
        var response = await api.GetFixturesAsync(ParseId(competitionExternalId), seasonYear, cancellationToken);
        ApiFootballException.ThrowIfErrors(response, "/fixtures");

        return response.Response.Select(ApiFootballMapper.ToFixture).ToList();
    }

    public async Task<ExternalMatchReport> GetMatchReportAsync(ExternalFixture fixture, CancellationToken cancellationToken = default)
    {
        if (!fixture.IsFinished)
        {
            throw new InvalidOperationException($"Fixture {fixture.ExternalId} is not finished and cannot be imported yet.");
        }

        var fixtureId = ParseId(fixture.ExternalId);

        // Sequencial de propósito: o pacer espaça as chamadas conforme o limite por minuto
        var players = await api.GetFixturePlayersAsync(fixtureId, cancellationToken);
        ApiFootballException.ThrowIfErrors(players, "/fixtures/players");

        var events = await api.GetFixtureEventsAsync(fixtureId, cancellationToken);
        ApiFootballException.ThrowIfErrors(events, "/fixtures/events");

        var lineups = await api.GetFixtureLineupsAsync(fixtureId, cancellationToken);
        ApiFootballException.ThrowIfErrors(lineups, "/fixtures/lineups");

        return ApiFootballMapper.ToMatchReport(fixture, players.Response, events.Response, lineups.Response);
    }

    private static int ParseId(string externalId) => int.Parse(externalId, CultureInfo.InvariantCulture);
}
