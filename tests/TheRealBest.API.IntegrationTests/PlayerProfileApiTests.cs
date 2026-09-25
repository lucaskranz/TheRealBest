namespace TheRealBest.API.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Players;
using TheRealBest.Application.DTOs.Ranking;

/// <summary>
/// Perfil do jogador contra o banco real (os testes com mocks não pegavam navegações não carregadas pelo EF).
/// </summary>
public class PlayerProfileApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task GetProfile_ForRankedPlayer_ReturnsMatchesAndAttributes()
    {
        var client = factory.CreateClient();
        var ranking = await client.GetFromJsonAsync<ApiResponse<List<SeasonRankingItemDto>>>("/api/v1/ranking?seasonYear=2023&pageSize=1", JsonOpts);
        if (ranking?.Data is not { Count: > 0 })
        {
            return; // banco sem dados
        }

        var response = await client.GetAsync($"/api/v1/players/{ranking.Data[0].PlayerId}?seasonYear=2023");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = (await response.Content.ReadFromJsonAsync<ApiResponse<PlayerProfileDto>>(JsonOpts))!.Data!;
        profile.RecentMatches.Should().NotBeEmpty();
        profile.RecentMatches.Should().OnlyContain(m => m.HomeTeamName.Length > 0 && m.AwayTeamName.Length > 0 && m.TeamName.Length > 0);
        profile.RecentMatches.Select(m => m.MatchDate).Should().BeInDescendingOrder();
        profile.Attributes.Should().HaveCount(6);
        profile.SeasonRanking!.TeamName.Should().NotBeNullOrEmpty();
    }
}
