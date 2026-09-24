namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TheRealBest.Application.DTOs;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;
using Xunit;

public class MatchDataIngestionPipelineTests
{
    private readonly Mock<IFootballDataProvider> _dataProvider = new();
    private readonly Mock<IIngestMatchDataUseCase> _ingestUseCase = new();
    private readonly Mock<IRecalculateSeasonRankingUseCase> _recalculateUseCase = new();

    [Fact]
    public async Task IngestCompetitionSeasonAsync_ShouldIngestFinishedMatchesAndRecalculate()
    {
        // Arrange
        var extComp = new ExternalCompetition("2", "Champions League", "Europe", CompetitionTier.UclKnockout, 2023);
        var home = new ExternalTeam("1", "Home", "logo");
        var away = new ExternalTeam("2", "Away", "logo");

        var finishedFixture = new ExternalFixture("fix-fin", extComp, home, away, "Final", true, DateTime.UtcNow, true, 2, 1);
        var unfinishedFixture = new ExternalFixture("fix-unfin", extComp, home, away, "Group", false, DateTime.UtcNow, false, null, null);

        _dataProvider.Setup(d => d.GetFixturesAsync("2", 2023, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExternalFixture> { finishedFixture, unfinishedFixture });

        var report = new ExternalMatchReport(finishedFixture, Array.Empty<ExternalPlayerPerformance>());
        _dataProvider.Setup(d => d.GetMatchReportAsync(finishedFixture, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        _ingestUseCase.Setup(u => u.ExecuteAsync(report, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IngestionResultDto(Guid.NewGuid(), "fix-fin", 22, 22, AlreadyExists: false));

        var pipeline = new MatchDataIngestionPipeline(
            _dataProvider.Object,
            _ingestUseCase.Object,
            _recalculateUseCase.Object,
            NullLogger<MatchDataIngestionPipeline>.Instance);

        // Act
        var results = await pipeline.IngestCompetitionSeasonAsync("2", 2023);

        // Assert
        results.Should().HaveCount(1);
        results[0].ExternalFixtureId.Should().Be("fix-fin");

        _dataProvider.Verify(d => d.GetMatchReportAsync(finishedFixture, It.IsAny<CancellationToken>()), Times.Once);
        _dataProvider.Verify(d => d.GetMatchReportAsync(unfinishedFixture, It.IsAny<CancellationToken>()), Times.Never);
        _recalculateUseCase.Verify(r => r.ExecuteAsync(2023, It.IsAny<CancellationToken>()), Times.Once);
    }
}