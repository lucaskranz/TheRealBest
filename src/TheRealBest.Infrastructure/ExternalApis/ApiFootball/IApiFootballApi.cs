namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using Refit;

/// <summary>
/// Endpoints da API-Football v3 (api-sports.io). Cada chamada consome 1 requisição da cota diária.
/// </summary>
public interface IApiFootballApi
{
    [Get("/fixtures")]
    Task<ApiFootballResponse<FixtureItem>> GetFixturesAsync(int league, int season, CancellationToken cancellationToken = default);

    [Get("/fixtures/players")]
    Task<ApiFootballResponse<FixturePlayersItem>> GetFixturePlayersAsync(int fixture, CancellationToken cancellationToken = default);

    [Get("/fixtures/events")]
    Task<ApiFootballResponse<EventItem>> GetFixtureEventsAsync(int fixture, CancellationToken cancellationToken = default);

    [Get("/fixtures/lineups")]
    Task<ApiFootballResponse<LineupItem>> GetFixtureLineupsAsync(int fixture, CancellationToken cancellationToken = default);
}
