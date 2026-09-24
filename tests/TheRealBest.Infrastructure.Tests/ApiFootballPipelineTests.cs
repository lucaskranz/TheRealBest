namespace TheRealBest.Infrastructure.Tests;

using System.Net;
using System.Text;
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

    private static ServiceProvider BuildServices(RecordedApi server, string apiKey = "test-key")
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=unused",
                ["ApiFootball:ApiKey"] = apiKey,
                ["ApiFootball:RequestsPerMinute"] = "60000",
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
