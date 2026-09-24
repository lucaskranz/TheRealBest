namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;
using MatchEntity = TheRealBest.Domain.Entities.Match;
using Xunit;

public class IngestMatchDataUseCaseTests
{
    private readonly Mock<ICompetitionRepository> _competitionRepo = new();
    private readonly Mock<ITeamRepository> _teamRepo = new();
    private readonly Mock<IPlayerRepository> _playerRepo = new();
    private readonly Mock<IMatchRepository> _matchRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IScoringEngine> _scoringEngine = new();

    private IngestMatchDataUseCase CreateUseCase() =>
        new(
            _competitionRepo.Object,
            _teamRepo.Object,
            _playerRepo.Object,
            _matchRepo.Object,
            _unitOfWork.Object,
            _scoringEngine.Object,
            NullLogger<IngestMatchDataUseCase>.Instance);

    [Fact]
    public async Task ExecuteAsync_WhenMatchNotIngested_ShouldProcessAndReturnSuccess()
    {
        // Arrange
        var extCompetition = new ExternalCompetition("ucl", "Champions League", "Europe", CompetitionTier.UclKnockout, 2023);
        var homeTeam = new ExternalTeam("mci", "Manchester City", "logo1");
        var awayTeam = new ExternalTeam("rma", "Real Madrid", "logo2");
        var fixture = new ExternalFixture("fix-1", extCompetition, homeTeam, awayTeam, "Final", true, DateTime.UtcNow, true, 2, 1);

        var player = new ExternalPlayer("rodri-1", "Rodri", "photo1");
        var statLine = new PlayerStatLine
        {
            Position = PlayerPosition.CDM,
            MinutesPlayed = 90,
            Goals = 1,
            PassesTotal = 80,
            PassesAccurate = 75,
            TacklesTotal = 4
        };

        var performance = new ExternalPlayerPerformance(player, "mci", statLine);
        var report = new ExternalMatchReport(fixture, new[] { performance });

        _matchRepo.Setup(m => m.GetByExternalIdAsync("fix-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchEntity?)null);

        _competitionRepo.Setup(c => c.GetByExternalIdAndSeasonAsync("ucl", 2023, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Competition?)null);

        _teamRepo.Setup(t => t.GetByExternalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);

        _playerRepo.Setup(p => p.GetByExternalIdAsync("rodri-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        var mockReceipt = new MatchReceipt(
            Guid.NewGuid(),
            Guid.NewGuid(),
            PlayerPosition.CDM,
            50m,
            Array.Empty<ActionScoreItem>(),
            Array.Empty<ActionScoreItem>(),
            SubtotalRaw: 35m,
            PositionBaseline: 15m,
            ContextMultiplier.Default,
            MinutesFactor: 1m,
            FinalMps: 70m,
            AlgorithmVersion: 1,
            CalculatedAt: DateTime.UtcNow,
            CountsTowardsSeason: true);

        _scoringEngine.Setup(s => s.CalculateMatchScore(It.IsAny<MatchPlayerStats>(), It.IsAny<MatchContext>()))
            .Returns(mockReceipt);

        var useCase = CreateUseCase();

        // Act
        var result = await useCase.ExecuteAsync(report);

        // Assert
        result.Should().NotBeNull();
        result.ExternalFixtureId.Should().Be("fix-1");
        result.AlreadyExists.Should().BeFalse();
        result.PlayersProcessed.Should().Be(1);
        result.ScoresCalculated.Should().Be(1);

        _competitionRepo.Verify(c => c.AddAsync(It.IsAny<Competition>(), It.IsAny<CancellationToken>()), Times.Once);
        _teamRepo.Verify(t => t.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _matchRepo.Verify(m => m.AddAsync(It.IsAny<MatchEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _playerRepo.Verify(p => p.AddAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()), Times.Once);
        _matchRepo.Verify(m => m.AddPlayerStatsAsync(It.IsAny<MatchPlayerStats>(), It.IsAny<CancellationToken>()), Times.Once);
        _matchRepo.Verify(m => m.AddPerformanceScoreAsync(It.IsAny<MatchPerformanceScore>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}