namespace TheRealBest.API.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Audit;
using TheRealBest.Application.DTOs.Matches;
using TheRealBest.Application.DTOs.Ranking;

public class MatchesAndAuditApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public MatchesAndAuditApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMatches_Returns200WithPagedEnvelope()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/matches?seasonYear=2023&page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<List<MatchSummaryDto>>>(JsonOpts);

        envelope.Should().NotBeNull();
        envelope!.Data.Should().NotBeNull();
        envelope.Meta.Should().NotBeNull();
        envelope.Meta!.Page.Should().Be(1);
        envelope.Errors.Should().BeNull();
    }

    [Fact]
    public async Task GetMatchDetail_And_AuditReceipt_FromRealSeededData()
    {
        // 1. Obter partidas existentes
        var matchesResponse = await _client.GetAsync("/api/v1/matches?seasonYear=2023&page=1&pageSize=5");
        matchesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var matchesEnvelope = await matchesResponse.Content.ReadFromJsonAsync<ApiResponse<List<MatchSummaryDto>>>(JsonOpts);

        if (matchesEnvelope?.Data is not null && matchesEnvelope.Data.Count > 0)
        {
            var matchId = matchesEnvelope.Data[0].Id;

            // 2. Detalhar a partida
            var detailResponse = await _client.GetAsync($"/api/v1/matches/{matchId}");
            detailResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var detailEnvelope = await detailResponse.Content.ReadFromJsonAsync<ApiResponse<MatchDetailDto>>(JsonOpts);

            detailEnvelope.Should().NotBeNull();
            detailEnvelope!.Data.Should().NotBeNull();
            detailEnvelope.Data!.Id.Should().Be(matchId);

            if (detailEnvelope.Data.PlayerPerformances.Count > 0)
            {
                var perf = detailEnvelope.Data.PlayerPerformances[0];
                var playerId = perf.PlayerId;

                // 3. Obter o extrato auditÃ¡vel da nota do jogador nesta partida
                var auditResponse = await _client.GetAsync($"/api/v1/audit/matches/{matchId}/players/{playerId}");
                auditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                var auditEnvelope = await auditResponse.Content.ReadFromJsonAsync<ApiResponse<MatchPerformanceReceiptDto>>(JsonOpts);

                auditEnvelope.Should().NotBeNull();
                auditEnvelope!.Data.Should().NotBeNull();
                auditEnvelope.Data!.PlayerId.Should().Be(playerId);
                auditEnvelope.Data.Formula.Should().NotBeNull();
                auditEnvelope.Data.Formula.BaseScore.Should().Be(50.0m);
            }
        }
    }

    [Fact]
    public async Task GetMatchAuditReceipt_WhenNotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/audit/matches/{Guid.NewGuid()}/players/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOpts);
        envelope.Should().NotBeNull();
        envelope!.Errors.Should().NotBeEmpty();
    }
}