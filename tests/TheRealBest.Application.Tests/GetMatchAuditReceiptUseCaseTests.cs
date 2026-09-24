namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Moq;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using MatchEntity = TheRealBest.Domain.Entities.Match;

public class GetMatchAuditReceiptUseCaseTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
    private readonly GetMatchAuditReceiptUseCase _useCase;

    public GetMatchAuditReceiptUseCaseTests()
    {
        _useCase = new GetMatchAuditReceiptUseCase(_matchRepositoryMock.Object, TestLabels.Create());
    }

    [Fact]
    public async Task ExecuteAsync_WhenScoreExists_ReturnsCompleteAuditReceipt()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var statsId = Guid.NewGuid();

        var player = new Player("p-rodri", "Rodri", "Spain", new DateOnly(1996, 6, 22), "https://img.com/rodri.png", PlayerPosition.CDM, id: playerId);
        var homeTeam = new Team("t-mci", "Manchester City", "MCI", "https://img.com/mci.png", "England", 2000);
        var awayTeam = new Team("t-ars", "Arsenal", "ARS", "https://img.com/ars.png", "England", 1900);
        var comp = new Competition("c-pl", "Premier League", "England", CompetitionTier.TopLeague, 1.0m, 2023);
        var match = new MatchEntity("m-1", comp.Id, homeTeam.Id, awayTeam.Id, "Matchday 5", DateTime.UtcNow, false, 2, 2, id: matchId);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.Competition))!.SetValue(match, comp);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.HomeTeam))!.SetValue(match, homeTeam);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.AwayTeam))!.SetValue(match, awayTeam);

        var stats = new MatchPlayerStats(matchId, playerId, homeTeam.Id, PlayerPosition.CDM, 90, id: statsId);

        var actionJson = "[{\"ActionKey\":\"goal\",\"Label\":\"goal\",\"Count\":1,\"UnitWeight\":25.0,\"TotalPoints\":25.0,\"Minute\":null,\"MinutesFactor\":1.0}]";
        var penaltyJson = "[{\"ActionKey\":\"yellow_card\",\"Label\":\"yellow_card\",\"Count\":1,\"UnitWeight\":-4.0,\"TotalPoints\":-4.0,\"Minute\":null,\"MinutesFactor\":1.0}]";

        var score = new MatchPerformanceScore(
            matchId: matchId,
            playerId: playerId,
            matchPlayerStatsId: statsId,
            positionEvaluated: PlayerPosition.CDM,
            baseScore: 50.0m,
            actionBreakdownJson: actionJson,
            penaltyBreakdownJson: penaltyJson,
            subtotalRaw: 21.0m,
            positionBaseline: 15.0m,
            tournamentMultiplier: 1.0m,
            opponentMultiplier: 1.1m,
            clutchMultiplier: 1.0m,
            contextMultiplierCombined: 1.1m,
            minutesFactor: 1.0m,
            finalMps: 56.6m,
            algorithmVersion: 1,
            countsTowardsSeason: true
        );

        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.Player))!.SetValue(score, player);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.Match))!.SetValue(score, match);
        typeof(MatchPerformanceScore).GetProperty(nameof(MatchPerformanceScore.MatchPlayerStats))!.SetValue(score, stats);

        _matchRepositoryMock
            .Setup(r => r.GetPerformanceScoreAsync(matchId, playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(score);

        // Act
        var receipt = await _useCase.ExecuteAsync(matchId, playerId);

        // Assert
        receipt.Should().NotBeNull();
        receipt!.PlayerName.Should().Be("Rodri");
        receipt.PlayerPosition.Should().Be("CDM");
        receipt.CompetitionName.Should().Be("Premier League");
        receipt.HomeTeamName.Should().Be("Manchester City");
        receipt.AwayTeamName.Should().Be("Arsenal");
        receipt.MinutesPlayed.Should().Be(90);

        receipt.Formula.BaseScore.Should().Be(50.0m);
        receipt.Formula.FinalMps.Should().Be(56.6m);
        receipt.Formula.SubtotalRaw.Should().Be(21.0m);

        receipt.PositiveActions.Should().HaveCount(1);
        receipt.PositiveActions[0].ActionKey.Should().Be("goal");
        receipt.PositiveActions[0].TotalPoints.Should().Be(25.0m);

        receipt.Penalties.Should().HaveCount(1);
        receipt.Penalties[0].ActionKey.Should().Be("yellow_card");
        receipt.Penalties[0].TotalPoints.Should().Be(-4.0m);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScoreNotFound_ReturnsNull()
    {
        // Arrange
        _matchRepositoryMock
            .Setup(r => r.GetPerformanceScoreAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchPerformanceScore?)null);

        // Act
        var receipt = await _useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        receipt.Should().BeNull();
    }
}