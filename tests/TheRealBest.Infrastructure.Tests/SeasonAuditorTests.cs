namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Infrastructure.Ingestion;

public class SeasonAuditorTests
{
    private readonly Mock<IFootballDataProvider> _provider = new();
    private readonly Mock<IMatchRepository> _matches = new();

    [Fact]
    public async Task Audit_ComparesFinishedFixturesWithStoredMatches()
    {
        _provider
            .Setup(p => p.GetFixturesAsync("39", 2023, It.IsAny<CancellationToken>()))
            .ReturnsAsync([Fixture("1"), Fixture("2"), Fixture("3"), Fixture("4"), Fixture("5", finished: false)]);
        _matches
            .Setup(m => m.GetImportAuditAsync(
                It.Is<IReadOnlyCollection<string>>(ids => ids.Order().SequenceEqual(new[] { "1", "2", "3", "4" })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new MatchImportAudit("1", true, "Arsenal", "Luton", 1950, 1500, PlayerCount: 30, TacticalPositionCount: 12),
                new MatchImportAudit("2", true, "Arsenal", "Luton", 1950, null, PlayerCount: 28, TacticalPositionCount: 0),
                new MatchImportAudit("3", true, "Luton", "Chelsea", null, null, PlayerCount: 0, TacticalPositionCount: 0),
            ]);

        var report = await new SeasonAuditor(_provider.Object, _matches.Object, NullLogger<SeasonAuditor>.Instance)
            .AuditAsync(2023, ApiFootballCompetitions.PremierLeague);

        var line = report.Competitions.Should().ContainSingle().Subject;
        line.Should().BeEquivalentTo(new
        {
            Expected = 4,
            Stored = 3,
            Missing = 1,
            WithoutPlayerData = 1,
            WithoutTacticalPositions = 1,
            MissingElo = 2,
        });
        report.TeamsMissingElo.Should().Equal(("Luton", 2), ("Chelsea", 1));
        report.IsComplete.Should().BeFalse();
    }

    [Fact]
    public async Task Audit_QuotaExhausted_MarksCompetitionUnavailable()
    {
        _provider
            .Setup(p => p.GetFixturesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiFootballException(ApiFootballErrorKind.DailyQuotaExceeded, "quota"));

        var report = await new SeasonAuditor(_provider.Object, _matches.Object, NullLogger<SeasonAuditor>.Instance)
            .AuditAsync(2023, ApiFootballCompetitions.PremierLeague);

        report.Competitions.Should().ContainSingle().Which.Available.Should().BeFalse();
        report.IsComplete.Should().BeFalse();
    }

    private static ExternalFixture Fixture(string id, bool finished = true) =>
        new(
            id,
            new ExternalCompetition("39", "Premier League", "England", CompetitionTier.TopLeague, 2023),
            new ExternalTeam("42", "Arsenal", string.Empty),
            new ExternalTeam("1359", "Luton", string.Empty),
            "Regular Season - 1",
            IsKnockout: false,
            new DateTime(2024, 1, 1, 15, 0, 0, DateTimeKind.Utc),
            finished,
            HomeScore: finished ? 1 : null,
            AwayScore: finished ? 0 : null);
}
