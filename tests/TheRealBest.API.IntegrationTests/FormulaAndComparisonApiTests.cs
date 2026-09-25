namespace TheRealBest.API.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TheRealBest.API.Formula;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Comparison;
using TheRealBest.Domain.Enums;
using TheRealBest.Scoring.Calculators;
using TheRealBest.Scoring.Rules;

public class FormulaAndComparisonApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public FormulaAndComparisonApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetFormula_ReflectsTheScoringEngine()
    {
        var formula = await GetData<FormulaDescriptor>("/api/v1/formula", "pt-BR");

        formula.AlgorithmVersion.Should().Be(ScoringRulesProvider.DefaultVersion);
        formula.Season.MinMatchesForRanking.Should().Be(SeasonScoreCalculator.MinMatchesForRanking);
        formula.Season.MinMinutesForRanking.Should().Be(SeasonScoreCalculator.MinMinutesForRanking);
        formula.Positions.Should().HaveCount(8);
        formula.Positions.Single(p => p.Code == "CM").SharesMatrixWith.Should().Be("CDM");
        formula.Positions.Single(p => p.Code == "ST").Baseline.Should().Be(ScoringRulesProvider.Default.BaselineFor(PlayerPosition.ST));

        var goal = formula.Actions.Single(a => a.Key == "goal");
        goal.Weights["ST"].Should().Be(ScoringRulesProvider.Default.For(PlayerPosition.ST).WeightFor(ActionType.Goal));
        goal.HasDataSource.Should().BeTrue();
        formula.Actions.Single(a => a.Key == "expected_assists").HasDataSource.Should().BeFalse();
    }

    [Fact]
    public async Task GetFormula_LocalizesLabels()
    {
        var formula = await GetData<FormulaDescriptor>("/api/v1/formula", "en");

        formula.Positions.Single(p => p.Code == "GK").Label.Should().Be("Goalkeeper");
        formula.Actions.Single(a => a.Key == "goal").Label.Should().Be("Goal");
    }

    [Fact]
    public async Task GetBallonDor2024_ReturnsAllNomineesWithAValidStatus()
    {
        var comparison = await GetData<BallonDorComparisonDto>("/api/v1/comparison/ballon-dor/2024", "pt-BR");

        comparison.SeasonYear.Should().Be(2023);
        comparison.Entries.Should().HaveCount(30);
        comparison.Entries.Should().OnlyContain(e =>
            e.Status == ComparisonStatus.Ranked || e.Status == ComparisonStatus.NotEligible || e.Status == ComparisonStatus.NoData);
        comparison.Entries.Where(e => e.Status == ComparisonStatus.Ranked).Should().OnlyContain(e => e.OurRank > 0 && e.FssScore != null);
        comparison.Entries.Where(e => e.Status == ComparisonStatus.NoData).Should().OnlyContain(e => e.FssScore == null);
    }

    [Fact]
    public async Task GetBallonDor_UnknownYear_Returns404WithLocalizedError()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/comparison/ballon-dor/1990");
        request.Headers.AcceptLanguage.ParseAdd("en");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<BallonDorComparisonDto>>(JsonOpts);
        envelope!.Errors.Should().ContainSingle().Which.Should().Contain("1990").And.StartWith("No official");
    }

    private async Task<T> GetData<T>(string url, string locale)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.AcceptLanguage.ParseAdd(locale);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOpts);
        envelope!.Data.Should().NotBeNull();
        return envelope.Data!;
    }
}
