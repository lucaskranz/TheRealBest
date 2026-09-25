namespace TheRealBest.Application.Tests;

using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TheRealBest.Application.DTOs;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;
using MatchEntity = TheRealBest.Domain.Entities.Match;
using Xunit;

public class BackfillEloUseCaseTests
{
    private const int Season = 2023;

    private readonly Mock<IMatchRepository> _matchRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IScoringEngine> _scoringEngine = new();
    private readonly Mock<IClubEloProvider> _clubElo = new();
    private readonly Mock<IRecalculateSeasonRankingUseCase> _recalculate = new();

    private BackfillEloUseCase CreateUseCase() =>
        new(_matchRepo.Object, _unitOfWork.Object, _scoringEngine.Object, _clubElo.Object, _recalculate.Object,
            NullLogger<BackfillEloUseCase>.Instance);

    [Fact]
    public async Task ExecuteAsync_FillsMissingEloAndRescoresTheMatch()
    {
        var (match, score) = ArrangeMatch(homeElo: null, awayElo: null);
        _clubElo.Setup(c => c.GetEloAsync("Arsenal", It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync(1950);
        _clubElo.Setup(c => c.GetEloAsync("Chelsea", It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync(1800);
        _scoringEngine.Setup(s => s.CalculateMatchScore(It.IsAny<MatchPlayerStats>(), It.IsAny<MatchContext>()))
            .Returns(Receipt(finalMps: 71.5m, opponentMultiplier: 1.08m));
        _recalculate.Setup(r => r.ExecuteAsync(Season, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SeasonRankingSummaryDto(Season, 1, 0, DateTime.UtcNow));

        var summary = await CreateUseCase().ExecuteAsync(Season);

        match.HomeEloRating.Should().Be(1950);
        match.AwayEloRating.Should().Be(1800);
        score.FinalMps.Should().Be(71.5m);
        score.OpponentMultiplier.Should().Be(1.08m);
        summary.Should().BeEquivalentTo(new { MatchesMissingElo = 1, MatchesUpdated = 1, PerformancesRescored = 1, TeamsNotFound = Array.Empty<string>() });
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _recalculate.Verify(r => r.ExecuteAsync(Season, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_KeepsKnownRatingAndSkipsMatchWhenNothingNewIsFound()
    {
        var (match, score) = ArrangeMatch(homeElo: 1950, awayElo: null);
        _clubElo.Setup(c => c.GetEloAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync((int?)null);

        var summary = await CreateUseCase().ExecuteAsync(Season);

        match.HomeEloRating.Should().Be(1950);
        score.FinalMps.Should().Be(60m);
        summary.MatchesUpdated.Should().Be(0);
        summary.TeamsNotFound.Should().Equal("Chelsea");
        _clubElo.Verify(c => c.GetEloAsync("Arsenal", It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        _scoringEngine.Verify(s => s.CalculateMatchScore(It.IsAny<MatchPlayerStats>(), It.IsAny<MatchContext>()), Times.Never);
        _recalculate.Verify(r => r.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private (MatchEntity Match, MatchPerformanceScore Score) ArrangeMatch(int? homeElo, int? awayElo)
    {
        var competition = new Competition("39", "Premier League", "England", CompetitionTier.TopLeague, 1.10m, Season);
        var home = new Team("42", "Arsenal", "ARS", "logo", "England");
        var away = new Team("49", "Chelsea", "CHE", "logo", "England");
        var match = new MatchEntity("1035000", competition.Id, home.Id, away.Id, "Regular Season - 10", new DateTime(2023, 10, 21, 16, 30, 0, DateTimeKind.Utc), false, 2, 2);
        SetProperty(match, nameof(MatchEntity.Competition), competition);
        SetProperty(match, nameof(MatchEntity.HomeTeam), home);
        SetProperty(match, nameof(MatchEntity.AwayTeam), away);
        match.SetEloRatings(homeElo, awayElo);

        var player = new Player("1100", "Declan Rice", "England", null, "photo", PlayerPosition.CDM);
        var stats = new MatchPlayerStats(match.Id, player.Id, home.Id, PlayerPosition.CDM, 90);
        var score = new MatchPerformanceScore(match.Id, player.Id, stats.Id, PlayerPosition.CDM, 50, "[]", "[]", 20, 10, 1.10m, 1m, 1m, 1.10m, 1m, 60m);
        AddToCollection(match, "_playerStats", stats);
        AddToCollection(match, "_performanceScores", score);

        _matchRepo.Setup(m => m.GetClubMatchIdsMissingEloAsync(Season, It.IsAny<CancellationToken>())).ReturnsAsync([match.Id]);
        _matchRepo.Setup(m => m.GetForRescoringAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync([match]);
        return (match, score);
    }

    private static MatchReceipt Receipt(decimal finalMps, decimal opponentMultiplier) =>
        new(Guid.NewGuid(), Guid.NewGuid(), PlayerPosition.CDM, 50m, [], [], 20m, 10m,
            new ContextMultiplier(1.10m, opponentMultiplier, 1m), 1m, finalMps, 2, DateTime.UtcNow);

    private static void SetProperty(object target, string name, object value) =>
        target.GetType().GetProperty(name)!.SetValue(target, value);

    private static void AddToCollection<T>(MatchEntity match, string field, T item) =>
        ((List<T>)typeof(MatchEntity).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(match)!).Add(item);
}
