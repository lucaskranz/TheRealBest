namespace TheRealBest.Infrastructure.Tests;

using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Infrastructure.Tests.Support;
using static TheRealBest.Infrastructure.Tests.Support.ApiFootballFixtures;

/// <summary>
/// Pipeline completo registrado no DI (Refit + pacer + autenticação), com a rede substituída por respostas gravadas.
/// </summary>
public class ApiFootballPipelineTests
{
    [Fact]
    public async Task GetFixtures_SendsApiKeyAndMapsResponse()
    {
        var server = new RecordedApi(request => Json(Raw(UclFinal2023, "fixtures")));
        using var provider = BuildServices(server);

        var fixtures = await provider.GetRequiredService<IFootballDataProvider>().GetFixturesAsync("2", 2022);

        fixtures.Should().ContainSingle().Which.ExternalId.Should().Be("1027909");
        var request = server.Requests.Should().ContainSingle().Subject;
        request.RequestUri!.PathAndQuery.Should().Be("/fixtures?league=2&season=2022");
        request.Headers.GetValues(ApiFootballAuthHandler.ApiKeyHeader).Should().Equal("test-key");
    }

    [Fact]
    public async Task GetMatchReport_UsesThreeRequests()
    {
        var server = new RecordedApi(request => Json(Raw(UclFinal2023, request.RequestUri!.AbsolutePath.Split('/')[^1])));
        using var provider = BuildServices(server);

        var report = await provider.GetRequiredService<IFootballDataProvider>().GetMatchReportAsync(Fixture(UclFinal2023));

        report.Performances.Should().HaveCount(29);
        server.Requests.Select(r => r.RequestUri!.AbsolutePath)
            .Should().Equal("/fixtures/players", "/fixtures/events", "/fixtures/lineups");
    }

    [Fact]
    public async Task SeasonOutsideFreePlan_ThrowsSeasonNotAvailable()
    {
        const string body = """
            {"get":"fixtures","parameters":{"league":"39","season":"2026"},
             "errors":{"plan":"Free plans do not have access to this season, try from 2022 to 2024."},
             "results":0,"paging":{"current":1,"total":1},"response":[]}
            """;
        using var provider = BuildServices(new RecordedApi(_ => Json(body)));

        var act = () => provider.GetRequiredService<IFootballDataProvider>().GetFixturesAsync("39", 2026);

        (await act.Should().ThrowAsync<ApiFootballException>()).Which.Kind.Should().Be(ApiFootballErrorKind.SeasonNotAvailable);
    }

    [Theory]
    [InlineData("requests", ApiFootballErrorKind.DailyQuotaExceeded)]
    [InlineData("rateLimit", ApiFootballErrorKind.RateLimited)]
    [InlineData("token", ApiFootballErrorKind.InvalidKey)]
    public async Task ErrorEnvelope_IsClassified(string errorKey, ApiFootballErrorKind expected)
    {
        var body = $$"""{"errors":{"{{errorKey}}":"message"},"results":0,"response":[]}""";
        using var provider = BuildServices(new RecordedApi(_ => Json(body)));

        var act = () => provider.GetRequiredService<IFootballDataProvider>().GetFixturesAsync("39", 2023);

        (await act.Should().ThrowAsync<ApiFootballException>()).Which.Kind.Should().Be(expected);
    }

    [Fact]
    public async Task Http429_IsNotRetriedAndThrowsRateLimited()
    {
        var server = new RecordedApi(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests));
        using var provider = BuildServices(server);

        var act = () => provider.GetRequiredService<IFootballDataProvider>().GetFixturesAsync("39", 2023);

        (await act.Should().ThrowAsync<ApiFootballException>()).Which.Kind.Should().Be(ApiFootballErrorKind.RateLimited);
        server.Requests.Should().ContainSingle();
    }

    [Fact]
    public async Task MissingApiKey_FailsBeforeCallingTheApi()
    {
        var server = new RecordedApi(_ => Json("{}"));
        using var provider = BuildServices(server, apiKey: "");

        var act = () => provider.GetRequiredService<IFootballDataProvider>().GetFixturesAsync("39", 2023);

        (await act.Should().ThrowAsync<ApiFootballException>()).Which.Kind.Should().Be(ApiFootballErrorKind.InvalidKey);
        server.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMatchReports_WithoutBatchMode_UsesThreeRequestsPerMatch()
    {
        var server = new RecordedApi(request => Json(Raw(UclFinal2023, request.RequestUri!.AbsolutePath.Split('/')[^1])));
        using var provider = BuildServices(server);

        var reports = await provider.GetRequiredService<IFootballDataProvider>().GetMatchReportsAsync([Fixture(UclFinal2023)]);

        reports.Should().ContainSingle().Which.Performances.Should().HaveCount(29);
        server.Requests.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetMatchReports_BatchMode_UsesOneRequestAndMapsLikeTheSeparateEndpoints()
    {
        var server = new RecordedApi(_ => Json(BatchBody(UclFinal2023)));
        using var provider = BuildServices(server, batch: true);

        var reports = await provider.GetRequiredService<IFootballDataProvider>().GetMatchReportsAsync([Fixture(UclFinal2023)]);

        server.Requests.Should().ContainSingle().Which.RequestUri!.PathAndQuery.Should().Be("/fixtures?ids=1027909");
        reports.Should().ContainSingle().Which.Should().BeEquivalentTo(Report(UclFinal2023));
    }

    [Fact]
    public async Task GetMatchReports_BatchMode_SplitsIntoRequestsOfTwentyAndSkipsMissingMatches()
    {
        var server = new RecordedApi(_ => Json("""{"errors":[],"results":0,"response":[]}"""));
        using var provider = BuildServices(server, batch: true);
        var fixtures = Enumerable.Range(1, 25).Select(i => Fixture(UclFinal2023) with { ExternalId = i.ToString() }).ToList();

        var reports = await provider.GetRequiredService<IFootballDataProvider>().GetMatchReportsAsync(fixtures);

        reports.Should().BeEmpty();
        server.Requests.Select(r => r.RequestUri!.Query.Split('-').Length).Should().Equal(20, 5);
    }

    [Fact]
    public async Task GetMatchReports_UnfinishedMatch_Throws()
    {
        using var provider = BuildServices(new RecordedApi(_ => Json("{}")), batch: true);
        var fixture = Fixture(UclFinal2023) with { IsFinished = false };

        var act = () => provider.GetRequiredService<IFootballDataProvider>().GetMatchReportsAsync([fixture]);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>Resposta de /fixtures?ids montada com as respostas gravadas dos endpoints separados.</summary>
    private static string BatchBody(string match)
    {
        var item = JsonNode.Parse(Raw(match, "fixtures"))!["response"]![0]!.DeepClone();
        item["players"] = JsonNode.Parse(Raw(match, "players"))!["response"]!.DeepClone();
        item["events"] = JsonNode.Parse(Raw(match, "events"))!["response"]!.DeepClone();
        item["lineups"] = JsonNode.Parse(Raw(match, "lineups"))!["response"]!.DeepClone();
        return new JsonObject { ["errors"] = new JsonArray(), ["results"] = 1, ["response"] = new JsonArray(item) }.ToJsonString();
    }

    private static ServiceProvider BuildServices(RecordedApi server, string apiKey = "test-key", bool batch = false)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=unused",
                ["ApiFootball:ApiKey"] = apiKey,
                ["ApiFootball:RequestsPerMinute"] = "60000",
                ["ApiFootball:BatchFixtureDetails"] = batch.ToString(),
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        services.AddHttpClient(DependencyInjection.ApiFootballHttpClientName).ConfigurePrimaryHttpMessageHandler(() => server);

        return services.BuildServiceProvider();
    }

    private static HttpResponseMessage Json(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class RecordedApi(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(respond(request));
        }
    }
}
