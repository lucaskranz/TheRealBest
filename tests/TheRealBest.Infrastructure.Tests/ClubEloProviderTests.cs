namespace TheRealBest.Infrastructure.Tests;

using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TheRealBest.Infrastructure.ExternalApis.ClubElo;

/// <summary>
/// CSV SINTÉTICO no formato documentado da API do ClubElo ("Rank,Club,Country,Level,Elo,From,To").
/// Os valores não são ratings reais; a API estava fora do ar quando a integração foi escrita.
/// </summary>
public class ClubEloProviderTests
{
    private const string SyntheticCsv = """
        Rank,Club,Country,Level,Elo,From,To
        1,Man City,ENG,1,2050.61,2024-05-20,2024-06-10
        2,Real Madrid,ESP,1,1990.4,2024-06-02,2024-08-18
        3,Bayern,GER,1,1950.2,2024-05-19,2024-08-16
        None,Atlético,ESP,1,1850.7,2024-05-26,2024-08-18
        """;

    [Theory]
    [InlineData("Manchester City", 2051)]  // apelido + arredondamento
    [InlineData("Real Madrid", 1990)]      // nome igual
    [InlineData("Bayern Munich", 1950)]    // apelido
    [InlineData("Atletico Madrid", 1851)]  // apelido + acento no CSV
    public async Task GetElo_MatchesApiFootballNames(string apiFootballName, int expected)
    {
        var provider = CreateProvider(_ => Csv(SyntheticCsv));

        var elo = await provider.GetEloAsync(apiFootballName, new DateOnly(2024, 6, 1));

        elo.Should().Be(expected);
    }

    [Fact]
    public async Task GetElo_UnknownClub_ReturnsNull()
    {
        var provider = CreateProvider(_ => Csv(SyntheticCsv));

        (await provider.GetEloAsync("Team That Does Not Exist", new DateOnly(2024, 6, 1))).Should().BeNull();
    }

    [Fact]
    public async Task GetElo_SourceUnavailable_ReturnsNullInsteadOfThrowing()
    {
        var provider = CreateProvider(_ => new HttpResponseMessage(HttpStatusCode.BadGateway));

        (await provider.GetEloAsync("Real Madrid", new DateOnly(2024, 6, 1))).Should().BeNull();
    }

    [Fact]
    public async Task GetElo_LoadsEachDateOnce()
    {
        var calls = 0;
        var provider = CreateProvider(_ =>
        {
            calls++;
            return Csv(SyntheticCsv);
        });

        await provider.GetEloAsync("Real Madrid", new DateOnly(2024, 6, 1));
        await provider.GetEloAsync("Bayern Munich", new DateOnly(2024, 6, 1));
        await provider.GetEloAsync("Real Madrid", new DateOnly(2024, 5, 8));

        calls.Should().Be(2);
    }

    [Fact]
    public void ParseCsv_UnexpectedFormat_ReturnsEmpty()
    {
        ClubEloProvider.ParseCsv("<html>502 Bad Gateway</html>").Should().BeEmpty();
    }

    private static ClubEloProvider CreateProvider(Func<HttpRequestMessage, HttpResponseMessage> respond) =>
        new(
            new HttpClient(new StubHandler(respond)) { BaseAddress = new Uri("http://api.clubelo.com/") },
            new ClubEloOptions(),
            NullLogger<ClubEloProvider>.Instance);

    private static HttpResponseMessage Csv(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "text/csv") };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(respond(request));
    }
}
