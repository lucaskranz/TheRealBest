namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;
using MatchEntity = TheRealBest.Domain.Entities.Match;
using Xunit;

public class RecalculateSeasonRankingUseCaseTests
{
    private readonly Mock<IMatchRepository> _matchRepo = new();
    private readonly Mock<IRankingRepository> _rankingRepo = new();
    private readonly Mock<IScoringEngine> _scoringEngine = new();

    private RecalculateSeasonRankingUseCase CreateUseCase() =>
        new(
            _matchRepo.Object,
            _rankingRepo.Object,
            _scoringEngine.Object,
            NullLogger<RecalculateSeasonRankingUseCase>.Instance);

    [Fact]
    public async Task ExecuteAsync_ShouldRankEligiblePlayersAndSetRanksCorrectly()
    {
        // Arrange
        int seasonYear = 2023;

        var competition = new Competition("ucl", "UCL", "Europe", CompetitionTier.UclKnockout, 1.35m, seasonYear);
        var home = new Team("t1", "Team 1", "T1", "logo", "Country");
        var away = new Team("t2", "Team 2", "T2", "logo", "Country");
        var match = new MatchEntity("m1", competition.Id, home.Id, away.Id, "Final", DateTime.UtcNow, true, 1, 0);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.Competition))!.SetValue(match, competition);

        var player1 = new Player("p1", "Player A", "Spain", null, "photo", PlayerPosition.CDM);
        var player2 = new Player("p2", "Player B", "Brazil", null, "photo", PlayerPosition.W);

        var stats1 = new MatchPlayerStats(match.Id, player1.Id, home.Id, PlayerPosition.CDM, 90);
        var stats2 = new MatchPlayerStats(match.Id, player2.Id, away.Id, PlayerPosition.W, 90);

        var score1 = new MatchPerformanceScore(match.Id, player1.Id, stats1.Id, PlayerPosition.CDM, 50, "[]", "[]", 30, 10, 1.35m, 1.1m, 1.25m, 1.85m, 1m, 85m);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.Match))!.SetValue(score1, match);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.Player))!.SetValue(score1, player1);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.MatchPlayerStats))!.SetValue(score1, stats1);

        var score2 = new MatchPerformanceScore(match.Id, player2.Id, stats2.Id, PlayerPosition.W, 50, "[]", "[]", 25, 10, 1.35m, 1.1m, 1m, 1.48m, 1m, 80m);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.Match))!.SetValue(score2, match);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.Player))!.SetValue(score2, player2);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.MatchPlayerStats))!.SetValue(score2, stats2);

        _matchRepo.Setup(m => m.GetSeasonScoresAsync(seasonYear, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MatchPerformanceScore> { score1, score2 });

        // Player A: FSS 82 (Rank 1)
        _scoringEngine.Setup(s => s.CalculateSeasonScore(It.Is<IEnumerable<MatchPerformanceScore>>(sc => sc.Any(x => x.PlayerId == player1.Id)), It.IsAny<int>()))
            .Returns(new SeasonScore(12, 85m, 1020m, 85m, 0.98m, 82.5m, IsRankingEligible: true));

        // Player B: FSS 78 (Rank 2)
        _scoringEngine.Setup(s => s.CalculateSeasonScore(It.Is<IEnumerable<MatchPerformanceScore>>(sc => sc.Any(x => x.PlayerId == player2.Id)), It.IsAny<int>()))
            .Returns(new SeasonScore(11, 80m, 880m, 80m, 0.95m, 76.0m, IsRankingEligible: true));

        IEnumerable<SeasonRanking>? capturedRankings = null;
        _rankingRepo.Setup(r => r.BulkUpsertRankingsAsync(It.IsAny<IEnumerable<SeasonRanking>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<SeasonRanking>, CancellationToken>((rankings, _) => capturedRankings = rankings)
            .Returns(Task.CompletedTask);

        var useCase = CreateUseCase();

        // Act
        var result = await useCase.ExecuteAsync(seasonYear);

        // Assert
        result.Should().NotBeNull();
        result.SeasonYear.Should().Be(seasonYear);
        result.TotalRanked.Should().Be(2);
        result.EligibleCount.Should().Be(2);

        capturedRankings.Should().NotBeNull();
        var list = capturedRankings!.ToList();

        var rankedP1 = list.First(r => r.PlayerId == player1.Id);
        rankedP1.OverallRank.Should().Be(1);
        rankedP1.PositionRank.Should().Be(1);

        var rankedP2 = list.First(r => r.PlayerId == player2.Id);
        rankedP2.OverallRank.Should().Be(2);
        rankedP2.PositionRank.Should().Be(1); // Winger #1
    }
}