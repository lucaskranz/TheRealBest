namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Moq;
using TheRealBest.Application.Comparison;
using TheRealBest.Application.DTOs.Comparison;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.Specifications;

public class GetBallonDorComparisonUseCaseTests
{
    private readonly Mock<IRankingRepository> _rankingRepositoryMock = new();
    private readonly GetBallonDorComparisonUseCase _useCase;

    public GetBallonDorComparisonUseCaseTests()
    {
        _useCase = new GetBallonDorComparisonUseCase(_rankingRepositoryMock.Object, TestLabels.Create());
    }

    [Fact]
    public async Task ExecuteAsync_UnknownYear_ReturnsNull()
    {
        var result = await _useCase.ExecuteAsync(1990);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_ClassifiesEachNomineeByDataAvailability()
    {
        // Rodri elegível (nosso 2º), Mbappé abaixo do corte, Lookman sem id externo, Kane sem ranking
        var rodri = Ranking("44", "Rodri", PlayerPosition.CDM, matches: 16, minutes: 1426, fss: 51.44m, rank: 2, eligible: true);
        var mbappe = Ranking("278", "Kylian Mbappé", PlayerPosition.W, matches: 1, minutes: 90, fss: 7.06m, rank: 0, eligible: false);
        var rodrygo = Ranking("10009", "Rodrygo", PlayerPosition.W, matches: 12, minutes: 950, fss: 31.8m, rank: 10, eligible: true);
        Setup(nominees: [rodri, mbappe], topOfIndex: [rodri, rodrygo], eligibleCount: 13);

        var result = await _useCase.ExecuteAsync(2024);

        result.Should().NotBeNull();
        result!.SeasonYear.Should().Be(2023);
        result.EligiblePlayers.Should().Be(13);
        result.Entries.Should().HaveCount(BallonDorCatalog.Edition2024.Nominees.Count);

        var rodriEntry = result.Entries.Single(e => e.Name == "Rodri");
        rodriEntry.Status.Should().Be(ComparisonStatus.Ranked);
        rodriEntry.OurRank.Should().Be(2);
        rodriEntry.RankDelta.Should().Be(-1); // 1º no júri, 2º no índice
        rodriEntry.PrimaryPositionLabel.Should().Be("pt-BR:CDM");

        var mbappeEntry = result.Entries.Single(e => e.Name == "Kylian Mbappé");
        mbappeEntry.Status.Should().Be(ComparisonStatus.NotEligible);
        mbappeEntry.OurRank.Should().BeNull();
        mbappeEntry.RankDelta.Should().BeNull();
        mbappeEntry.TotalMatches.Should().Be(1);

        result.Entries.Single(e => e.Name == "Ademola Lookman").Status.Should().Be(ComparisonStatus.NoData);
        result.Entries.Single(e => e.Name == "Harry Kane").Status.Should().Be(ComparisonStatus.NoData);

        result.Unnominated.Should().ContainSingle().Which.Name.Should().Be("Rodrygo");
    }

    [Fact]
    public async Task ExecuteAsync_OnlyQueriesNomineesWithExternalIds()
    {
        Setup(nominees: [], topOfIndex: [], eligibleCount: 0);

        await _useCase.ExecuteAsync(2024);

        var expectedIds = BallonDorCatalog.Edition2024.Nominees
            .Where(n => n.ExternalApiId is not null)
            .Select(n => n.ExternalApiId!)
            .ToList();
        _rankingRepositoryMock.Verify(r => r.GetByExternalPlayerIdsAsync(
            2023,
            It.Is<IReadOnlyCollection<string>>(ids => ids.SequenceEqual(expectedIds)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Edition2024_HasThirtyNomineesInOfficialOrder()
    {
        var nominees = BallonDorCatalog.Edition2024.Nominees;

        nominees.Should().HaveCount(30);
        nominees.Select(n => n.OfficialRank).Should().BeInAscendingOrder();
        nominees[0].Name.Should().Be("Rodri");
        nominees.Where(n => n.ExternalApiId is not null).Select(n => n.ExternalApiId).Should().OnlyHaveUniqueItems();
    }

    private void Setup(IReadOnlyList<SeasonRanking> nominees, IReadOnlyList<SeasonRanking> topOfIndex, int eligibleCount)
    {
        _rankingRepositoryMock
            .Setup(r => r.GetByExternalPlayerIdsAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nominees);
        _rankingRepositoryMock
            .Setup(r => r.GetRankingsAsync(It.IsAny<PlayerRankingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(topOfIndex);
        _rankingRepositoryMock
            .Setup(r => r.CountRankingsAsync(It.IsAny<PlayerRankingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(eligibleCount);
    }

    private static SeasonRanking Ranking(
        string externalId, string name, PlayerPosition position, int matches, int minutes, decimal fss, int rank, bool eligible)
    {
        var player = new Player(externalId, name, "Spain", null, $"https://img/{externalId}.png", position);
        var ranking = new SeasonRanking(player.Id, 2023, matches, minutes, fss, fss * matches, 0.8m, fss, rank, rank, 1m, eligible);
        typeof(SeasonRanking).GetProperty(nameof(SeasonRanking.Player))!.SetValue(ranking, player);
        return ranking;
    }
}
