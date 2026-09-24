namespace TheRealBest.API.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Players;
using TheRealBest.Application.DTOs.Ranking;

public class RankingAndPlayersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public RankingAndPlayersApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRankings_Returns200WithStandardEnvelope()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/ranking?seasonYear=2023&onlyEligible=true&page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<List<SeasonRankingItemDto>>>(JsonOpts);

        envelope.Should().NotBeNull();
        envelope!.Data.Should().NotBeNull();
        envelope.Meta.Should().NotBeNull();
        envelope.Meta!.Page.Should().Be(1);
        envelope.Errors.Should().BeNull();

        // Com dados no banco, a lista vem ordenada pela posição e só com elegíveis (sem supor quem é o líder)
        if (envelope.Data!.Count > 0)
        {
            envelope.Data[0].OverallRank.Should().Be(1);
            envelope.Data.Select(r => r.OverallRank).Should().BeInAscendingOrder();
            envelope.Data.Should().OnlyContain(r => r.IsRankingEligible);
        }
    }

    [Fact]
    public async Task GetTopContenders_Returns200WithTopPlayers()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/ranking/top?seasonYear=2023&count=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<List<SeasonRankingItemDto>>>(JsonOpts);

        envelope.Should().NotBeNull();
        envelope!.Data.Should().NotBeNull();
        envelope.Data!.Count.Should().BeLessThanOrEqualTo(3);
    }

    [Fact]
    public async Task SearchPlayers_Returns200WithList()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/players?query=Rodri");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<List<PlayerSummaryDto>>>(JsonOpts);

        envelope.Should().NotBeNull();
        envelope!.Data.Should().NotBeNull();
        if (envelope.Data!.Count > 0)
        {
            envelope.Data.Should().Contain(p => p.Name.Contains("Rodri"));
        }
    }

    [Fact]
    public async Task GetPlayerProfile_WhenNotFound_Returns404Envelope()
    {
        // Act
        var nonExistentId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/v1/players/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOpts);

        envelope.Should().NotBeNull();
        envelope!.Data.Should().BeNull();
        envelope.Errors.Should().NotBeEmpty();
    }
}