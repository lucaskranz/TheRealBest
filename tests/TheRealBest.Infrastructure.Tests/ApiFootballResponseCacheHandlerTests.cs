namespace TheRealBest.Infrastructure.Tests;

using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using static TheRealBest.Infrastructure.Tests.Support.ApiFootballFixtures;

public sealed class ApiFootballResponseCacheHandlerTests : IDisposable
{
    private readonly string _cacheDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "trb-cache-tests", Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_cacheDirectory))
        {
            Directory.Delete(_cacheDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task FinishedMatchDetails_AreServedFromDiskOnSecondRequest()
    {
        var network = new CountingHandler(Raw(UclFinal2023, "players"));
        using var client = CreateClient(network);

        var first = await client.GetStringAsync("/fixtures/players?fixture=1027909");
        var second = await client.GetStringAsync("/fixtures/players?fixture=1027909");

        network.Calls.Should().Be(1);
        second.Should().Be(first);
        File.Exists(System.IO.Path.Combine(_cacheDirectory, "fixtures_players_fixture-1027909.json")).Should().BeTrue();
    }

    [Fact]
    public async Task ListWithOnlyFinishedFixtures_IsCached()
    {
        var network = new CountingHandler(Raw(UclFinal2023, "fixtures"));
        using var client = CreateClient(network);

        await client.GetStringAsync("/fixtures?league=2&season=2022");
        await client.GetStringAsync("/fixtures?league=2&season=2022");

        network.Calls.Should().Be(1);
    }

    [Fact]
    public async Task ListWithUpcomingFixture_IsNotCached()
    {
        var upcoming = Raw(UclFinal2023, "fixtures").Replace("\"short\":\"FT\"", "\"short\":\"NS\"");
        var network = new CountingHandler(upcoming);
        using var client = CreateClient(network);

        await client.GetStringAsync("/fixtures?league=2&season=2026");
        await client.GetStringAsync("/fixtures?league=2&season=2026");

        network.Calls.Should().Be(2);
    }

    [Fact]
    public async Task ErrorEnvelope_IsNotCached()
    {
        var network = new CountingHandler("""{"errors":{"plan":"Free plans do not have access to this season"},"results":0,"response":[]}""");
        using var client = CreateClient(network);

        await client.GetStringAsync("/fixtures?league=39&season=2026");
        await client.GetStringAsync("/fixtures?league=39&season=2026");

        network.Calls.Should().Be(2);
        Directory.Exists(_cacheDirectory).Should().BeFalse();
    }

    [Fact]
    public void CacheFileName_IsDerivedFromPathAndQuery()
    {
        ApiFootballResponseCacheHandler.CacheFileName(new Uri("https://x/fixtures?league=2&season=2023"))
            .Should().Be("fixtures_league-2_season-2023.json");
    }

    private HttpClient CreateClient(HttpMessageHandler network)
    {
        var options = Options.Create(new ApiFootballOptions { ResponseCacheEnabled = true, ResponseCacheDirectory = _cacheDirectory });
        return new HttpClient(new ApiFootballResponseCacheHandler(options) { InnerHandler = network })
        {
            BaseAddress = new Uri("https://v3.football.api-sports.io"),
        };
    }

    private sealed class CountingHandler(string body) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            });
        }
    }
}
