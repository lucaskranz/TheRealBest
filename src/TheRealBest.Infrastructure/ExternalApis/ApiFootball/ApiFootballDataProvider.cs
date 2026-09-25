namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;

/// <summary>
/// Implementação de <see cref="IFootballDataProvider"/> sobre a API-Football.
/// Custo em requisições: 1 por listagem de partidas; 3 por relatório de partida (jogadores, eventos, escalações),
/// ou 1 a cada <see cref="MaxFixturesPerBatch"/> partidas com <see cref="ApiFootballOptions.BatchFixtureDetails"/>.
/// </summary>
public sealed class ApiFootballDataProvider(
    IApiFootballApi api,
    IOptions<ApiFootballOptions> options,
    ILogger<ApiFootballDataProvider> logger) : IFootballDataProvider
{
    /// <summary>Limite de ids por requisição imposto pela API.</summary>
    public const int MaxFixturesPerBatch = 20;

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
        EnsureFinished(fixture);
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

    /// <summary>
    /// Relatórios das partidas pedidas, na mesma ordem. No modo em lote, partidas que a API não devolver ficam de fora.
    /// </summary>
    public async Task<IReadOnlyList<ExternalMatchReport>> GetMatchReportsAsync(
        IReadOnlyList<ExternalFixture> fixtures,
        CancellationToken cancellationToken = default)
    {
        foreach (var fixture in fixtures)
        {
            EnsureFinished(fixture);
        }

        if (!options.Value.BatchFixtureDetails)
        {
            var reports = new List<ExternalMatchReport>(fixtures.Count);
            foreach (var fixture in fixtures)
            {
                reports.Add(await GetMatchReportAsync(fixture, cancellationToken));
            }

            return reports;
        }

        var result = new List<ExternalMatchReport>(fixtures.Count);
        foreach (var batch in fixtures.Chunk(MaxFixturesPerBatch))
        {
            var ids = string.Join('-', batch.Select(f => f.ExternalId));
            var response = await api.GetFixturesByIdsAsync(ids, cancellationToken);
            ApiFootballException.ThrowIfErrors(response, "/fixtures?ids");

            var details = response.Response.ToDictionary(d => d.Fixture.Id.ToString(CultureInfo.InvariantCulture));
            foreach (var fixture in batch)
            {
                if (!details.TryGetValue(fixture.ExternalId, out var detail))
                {
                    continue;
                }

                try
                {
                    result.Add(ApiFootballMapper.ToMatchReport(fixture, detail.Players ?? [], detail.Events ?? [], detail.Lineups ?? []));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or NullReferenceException or KeyNotFoundException)
                {
                    // Resposta fora do formato esperado: a partida fica pendente (fora do resultado) e o lote segue
                    logger.LogError(ex, "Partida {FixtureId} ({Home} × {Away}) com dados inesperados na fonte; não foi importada.",
                        fixture.ExternalId, fixture.HomeTeam.Name, fixture.AwayTeam.Name);
                }
            }
        }

        return result;
    }

    private static void EnsureFinished(ExternalFixture fixture)
    {
        if (!fixture.IsFinished)
        {
            throw new InvalidOperationException($"Fixture {fixture.ExternalId} is not finished and cannot be imported yet.");
        }
    }

    private static int ParseId(string externalId) => int.Parse(externalId, CultureInfo.InvariantCulture);
}
