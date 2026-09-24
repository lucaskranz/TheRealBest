namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Moq;
using TheRealBest.Application.DTOs.Ranking;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.Specifications;

public class GetSeasonRankingUseCaseTests
{
    private readonly Mock<IRankingRepository> _rankingRepositoryMock = new();
    private readonly GetSeasonRankingUseCase _useCase;

    public GetSeasonRankingUseCaseTests()
    {
        _useCase = new GetSeasonRankingUseCase(_rankingRepositoryMock.Object, TestLabels.Create());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsPagedRankingsWithDeserializedTopMatches()
    {
        // Arrange
        var player = new Player("p-rodri", "Rodri", "Spain", new DateOnly(1996, 6, 22), "https://img.com/rodri.png", PlayerPosition.CDM);
        var ranking = new SeasonRanking(
            playerId: player.Id,
            seasonYear: 2023,
            totalMatches: 16,
            totalMinutes: 1426,
            mpsAverage: 47.25m,
            mpsSumWeighted: 800m,
            presenceFactor: 0.8m,
            fssScore: 38.13m,
            overallRank: 1,
            positionRank: 1,
            clutchIndex: 1.034m,
            isRankingEligible: true,
            top5MatchesJson: "[{\"MatchId\":\"00000000-0000-0000-0000-000000000001\",\"Date\":\"2024-05-19T15:00:00Z\",\"Phase\":\"Regular\",\"Mps\":65.25}]"
        );

        // Injeta player via reflexÃ£o
        typeof(SeasonRanking).GetProperty(nameof(SeasonRanking.Player))!
            .SetValue(ranking, player);

        _rankingRepositoryMock
            .Setup(r => r.GetRankingsAsync(It.IsAny<PlayerRankingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([ranking]);

        _rankingRepositoryMock
            .Setup(r => r.CountRankingsAsync(It.IsAny<PlayerRankingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var filter = new RankingFilterDto(SeasonYear: 2023, Page: 1, PageSize: 10);

        // Act
        var result = await _useCase.ExecuteAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);

        var item = result.Items[0];
        item.PlayerName.Should().Be("Rodri");
        item.Nationality.Should().Be("Spain");
        item.PrimaryPosition.Should().Be("CDM");
        item.OverallRank.Should().Be(1);
        item.PositionRank.Should().Be(1);
        item.FssScore.Should().Be(38.13m);
        item.IsRankingEligible.Should().BeTrue();
        item.TopMatches.Should().HaveCount(1);
        item.TopMatches[0].Mps.Should().Be(65.25m);
        item.TopMatches[0].Phase.Should().Be("Regular");
    }

    [Fact]
    public async Task GetTopContendersAsync_ReturnsTopNContenders()
    {
        // Arrange
        var player = new Player("p-vini", "Vinicius Junior", "Brazil", new DateOnly(2000, 7, 12), "https://img.com/vini.png", PlayerPosition.W);
        var ranking = new SeasonRanking(
            playerId: player.Id,
            seasonYear: 2023,
            totalMatches: 12,
            totalMinutes: 1050,
            mpsAverage: 38.37m,
            mpsSumWeighted: 600m,
            presenceFactor: 0.69m,
            fssScore: 26.11m,
            overallRank: 3,
            positionRank: 1,
            clutchIndex: 1.142m,
            isRankingEligible: true
        );

        typeof(SeasonRanking).GetProperty(nameof(SeasonRanking.Player))!
            .SetValue(ranking, player);

        _rankingRepositoryMock
            .Setup(r => r.GetRankingsAsync(It.Is<PlayerRankingSpec>(s => s.Take == 5), It.IsAny<CancellationToken>()))
            .ReturnsAsync([ranking]);

        // Act
        var contenders = await _useCase.GetTopContendersAsync(2023, 5);

        // Assert
        contenders.Should().HaveCount(1);
        contenders[0].PlayerName.Should().Be("Vinicius Junior");
        contenders[0].OverallRank.Should().Be(3);
    }
}