namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Moq;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using MatchEntity = TheRealBest.Domain.Entities.Match;

public class GetPlayerProfileUseCaseTests
{
    private readonly Mock<IPlayerRepository> _playerRepositoryMock = new();
    private readonly Mock<IRankingRepository> _rankingRepositoryMock = new();
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
    private readonly GetPlayerProfileUseCase _useCase;

    public GetPlayerProfileUseCaseTests()
    {
        _useCase = new GetPlayerProfileUseCase(
            _playerRepositoryMock.Object,
            _rankingRepositoryMock.Object,
            _matchRepositoryMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlayerExists_ReturnsProfileWithHistoryAndRanking()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var player = new Player("p-jude", "Jude Bellingham", "England", new DateOnly(2003, 6, 29), "https://img.com/jude.png", PlayerPosition.CAM, id: playerId);

        var ranking = new SeasonRanking(
            playerId: playerId,
            seasonYear: 2023,
            totalMatches: 12,
            totalMinutes: 1022,
            mpsAverage: 40.66m,
            mpsSumWeighted: 700m,
            presenceFactor: 0.68m,
            fssScore: 26.83m,
            overallRank: 2,
            positionRank: 1,
            clutchIndex: 1.162m,
            isRankingEligible: true
        );

        var homeTeam = new Team("t-rma", "Real Madrid", "RMA", "https://img.com/rma.png", "Spain", 1950);
        var awayTeam = new Team("t-bar", "Barcelona", "BAR", "https://img.com/bar.png", "Spain", 1880);
        var comp = new Competition("c-laliga", "La Liga", "Spain", CompetitionTier.TopLeague, 1.0m, 2023);
        var match = new MatchEntity("m-1", comp.Id, homeTeam.Id, awayTeam.Id, "Round 11", DateTime.UtcNow, false, 2, 1);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.Competition))!.SetValue(match, comp);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.HomeTeam))!.SetValue(match, homeTeam);
        typeof(MatchEntity).GetProperty(nameof(MatchEntity.AwayTeam))!.SetValue(match, awayTeam);

        var stats = new MatchPlayerStats(match.Id, playerId, homeTeam.Id, PlayerPosition.CAM, 90);
        stats.SetOffensiveStats(2, 0, 4, 3, 2, 1, 0, 0.8m, 0.2m, 3);
        typeof(MatchPlayerStats).GetProperty(nameof(MatchPlayerStats.Match))!.SetValue(stats, match);

        var perfScore = new MatchPerformanceScore(
            matchId: match.Id,
            playerId: playerId,
            matchPlayerStatsId: stats.Id,
            positionEvaluated: PlayerPosition.CAM,
            baseScore: 50.0m,
            actionBreakdownJson: "[]",
            penaltyBreakdownJson: "[]",
            subtotalRaw: 45.0m,
            positionBaseline: 15.0m,
            tournamentMultiplier: 1.0m,
            opponentMultiplier: 1.05m,
            clutchMultiplier: 1.25m,
            contextMultiplierCombined: 1.3125m,
            minutesFactor: 1.0m,
            finalMps: 89.37m,
            algorithmVersion: 1,
            countsTowardsSeason: true
        );
        typeof(MatchPlayerStats).GetProperty(nameof(MatchPlayerStats.PerformanceScore))!.SetValue(stats, perfScore);

        _playerRepositoryMock
            .Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _rankingRepositoryMock
            .Setup(r => r.GetByPlayerAndSeasonAsync(playerId, 2023, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ranking);

        _matchRepositoryMock
            .Setup(r => r.GetPlayerStatsByPlayerAndSeasonAsync(playerId, 2023, It.IsAny<CancellationToken>()))
            .ReturnsAsync([stats]);

        // Act
        var result = await _useCase.ExecuteAsync(playerId, 2023);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Jude Bellingham");
        result.Nationality.Should().Be("England");
        result.PrimaryPosition.Should().Be("CAM");
        result.SeasonRanking.Should().NotBeNull();
        result.SeasonRanking!.OverallRank.Should().Be(2);
        result.SeasonRanking.FssScore.Should().Be(26.83m);
        result.RecentMatches.Should().HaveCount(1);
        result.RecentMatches[0].Goals.Should().Be(2);
        result.RecentMatches[0].HomeScore.Should().Be(2);
        result.RecentMatches[0].AwayScore.Should().Be(1);
        result.RecentMatches[0].FinalMps.Should().Be(89.37m);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlayerNotFound_ReturnsNull()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        _playerRepositoryMock
            .Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        // Act
        var result = await _useCase.ExecuteAsync(playerId, 2023);

        // Assert
        result.Should().BeNull();
    }
}