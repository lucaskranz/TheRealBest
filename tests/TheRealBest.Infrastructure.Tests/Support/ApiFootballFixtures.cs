namespace TheRealBest.Infrastructure.Tests.Support;

using System.Text.Json;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <summary>
/// Respostas reais da API-Football gravadas em Fixtures/ApiFootball.
/// </summary>
public static class ApiFootballFixtures
{
    /// <summary>Final da UCL 2023: Manchester City 1 × 0 Inter (fixture 1027909).</summary>
    public const string UclFinal2023 = "ucl-2023-final";

    /// <summary>Quartas da UCL 2024 (volta): Manchester City 1 × 1 Real Madrid, prorrogação e pênaltis (fixture 1184780).</summary>
    public const string UclQuarterFinal2024 = "ucl-2024-qf-city-real";

    public static string Path(string match, string endpoint) =>
        System.IO.Path.Combine(AppContext.BaseDirectory, "Fixtures", "ApiFootball", $"{match}.{endpoint}.json");

    public static string Raw(string match, string endpoint) => File.ReadAllText(Path(match, endpoint));

    public static ApiFootballResponse<T> Load<T>(string match, string endpoint) =>
        JsonSerializer.Deserialize<ApiFootballResponse<T>>(Raw(match, endpoint), ApiFootballJson.Options)!;

    public static ExternalFixture Fixture(string match) =>
        ApiFootballMapper.ToFixture(Load<FixtureItem>(match, "fixtures").Response.Single());

    public static ExternalMatchReport Report(string match) =>
        ApiFootballMapper.ToMatchReport(
            Fixture(match),
            Load<FixturePlayersItem>(match, "players").Response,
            Load<EventItem>(match, "events").Response,
            Load<LineupItem>(match, "lineups").Response);
}

/// <summary>
/// Eventos sintéticos para cenários que as partidas gravadas não cobrem (gol contra, VAR, ordem invertida).
/// </summary>
public static class Events
{
    public static EventItem Goal(int minute, int teamId, int playerId, string detail = "Normal Goal", int? extra = null) =>
        Event(minute, extra, teamId, playerId, null, "Goal", detail);

    public static EventItem Substitution(int minute, int teamId, int outId, int inId) =>
        Event(minute, null, teamId, outId, inId, "subst", "Substitution 1");

    public static EventItem RedCard(int minute, int teamId, int playerId) =>
        Event(minute, null, teamId, playerId, null, "Card", "Red Card");

    public static EventItem VarGoalCancelled(int minute, int teamId, int playerId) =>
        Event(minute, null, teamId, playerId, null, "Var", "Goal cancelled");

    private static EventItem Event(int minute, int? extra, int teamId, int playerId, int? assistId, string type, string detail) =>
        new(
            new EventTime(minute, extra),
            new TeamInfo(teamId, $"Team {teamId}", null),
            new EventPerson(playerId, $"Player {playerId}"),
            new EventPerson(assistId, assistId is null ? null : $"Player {assistId}"),
            type,
            detail,
            Comments: null);
}
