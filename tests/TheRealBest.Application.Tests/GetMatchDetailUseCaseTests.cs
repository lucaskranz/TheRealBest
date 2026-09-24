namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Moq;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using MatchEntity = TheRealBest.Domain.Entities.Match;

public class GetMatchDetailUseCaseTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
    private readonly GetMatchDetailUseCase _useCase;

    public GetMatchDetailUseCaseTests()
    {
        _useCase = new GetMatchDetailUseCase(_matchRepositoryMock.Object, TestLabels.Create());
    }

    [Fact]
    public async Task ExecuteAsync_WhenMatchExists_ReturnsMatchDetailWithPerformances()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var homeTeam = new Team("t-rma", "Real Madrid", "RMA", "https://img.com/rma.png", "Spain", 1950);
        var awayTeam = new Team("t-dor", "Borussia Dortmund", "BVB", "https://img.com/bvb.png", "Germany", 1850);
        var comp = new Competition("c-ucl", "UEFA Champions League", "Europe", CompetitionTier.UclKnockout, 1.35m, 2023);
        var match = new MatchEntity("m-final", comp.Id, homeTeam.Id, awayTeam.Id, "Final", DateTime.UtcNow, true, 2, 0, id: matchId);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.Competition))!.SetValue(match, comp);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.HomeTeam))!.SetValue(match, homeTeam);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.AwayTeam))!.SetValue(match, awayTeam);

        var player = new Player("p-carvajal", "Dani Carvajal", "Spain", new DateOnly(1992, 1, 11), "https://img.com/carva.png", PlayerPosition.FB);
        var stats = new MatchPlayerStats(matchId, player.Id, homeTeam.Id, PlayerPosition.FB, 90);
        stats.SetOffensiveStats(1, 0, 1, 1, 0, 0, 0, 0.1m, 0m, 1);
        typeof(MatchPlayerStats).GetProperty(nameof(MatchPlayerStats.Player))!.SetValue(stats, player);
        typeof(MatchPlayerStats).GetProperty(nameof(MatchPlayerStats.Team))!.SetValue(stats, homeTeam);

        var score = new MatchPerformanceScore(
            matchId: matchId,
            playerId: player.Id,
            matchPlayerStatsId: stats.Id,
            positionEvaluated: PlayerPosition.FB,
            baseScore: 50.0m,
            actionBreakdownJson: "[]",
            penaltyBreakdownJson: "[]",
            subtotalRaw: 35.0m,
            positionBaseline: 15.0m,
            tournamentMultiplier: 1.35m,
            opponentMultiplier: 1.05m,
            clutchMultiplier: 1.25m,
            contextMultiplierCombined: 1.77m,
            minutesFactor: 1.0m,
            finalMps: 85.4m
        );

        // Reflection to add to private lists
        var statsList = (List<MatchPlayerStats>)typeof(MatchEntity).GetField("_playerStats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(match)!;
        statsList.Add(stats);

        var scoresList = (List<MatchPerformanceScore>)typeof(MatchEntity).GetField("_performanceScores", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(match)!;
        scoresList.Add(score);

        _matchRepositoryMock
            .Setup(r => r.GetWithStatsByIdAsync(matchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(match);

        // Act
        var detail = await _useCase.ExecuteAsync(matchId);

        // Assert
        detail.Should().NotBeNull();
        detail!.RoundPhase.Should().Be("Final");
        detail.IsKnockout.Should().BeTrue();
        detail.HomeScore.Should().Be(2);
        detail.AwayScore.Should().Be(0);
        detail.PlayerPerformances.Should().HaveCount(1);
        detail.PlayerPerformances[0].PlayerName.Should().Be("Dani Carvajal");
        detail.PlayerPerformances[0].FinalMps.Should().Be(85.4m);
    }
}