namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TheRealBest.Application.DTOs;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Infrastructure.Ingestion;

public class SeasonImporterTests
{
    private readonly Mock<IFootballDataProvider> _provider = new();
    private readonly Mock<IMatchRepository> _matches = new();
    private readonly Mock<IIngestMatchDataUseCase> _ingest = new();
    private readonly Mock<IRecalculateSeasonRankingUseCase> _recalculate = new();
    private readonly List<string> _existing = [];

    public SeasonImporterTests()
    {
        _provider
            .Setup(p => p.GetFixturesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _provider
            .Setup(p => p.GetMatchReportsAsync(It.IsAny<IReadOnlyList<ExternalFixture>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ExternalFixture> fixtures, CancellationToken _) =>
                fixtures.Select(f => new ExternalMatchReport(f, [Performance()])).ToList());
        _matches
            .Setup(m => m.GetExistingExternalIdsAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<string> ids, CancellationToken _) => ids.Where(_existing.Contains).ToHashSet());
        _recalculate
            .Setup(r => r.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SeasonRankingSummaryDto(2023, 0, 0, DateTime.UtcNow));
    }

    [Fact]
    public async Task Import_SkipsExistingAndUnfinishedMatches_AndRecalculatesTheSeason()
    {
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023,
            Fixture("1", 2023), Fixture("2", 2023), Fixture("3", 2023, finished: false));
        _existing.Add("1");

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2023, ApiFootballCompetitions.PremierLeague));

        summary.Stop.Should().Be(SeasonImportStop.Completed);
        var line = summary.Competitions.Should().ContainSingle().Subject;
        line.Listed.Should().Be(3);
        line.Finished.Should().Be(2);
        line.AlreadyImported.Should().Be(1);
        line.Imported.Should().Be(1);
        line.Pending.Should().Be(0);
        _ingest.Verify(i => i.ExecuteAsync(It.Is<ExternalMatchReport>(r => r.Fixture.ExternalId == "2"), It.IsAny<CancellationToken>()), Times.Once);
        _ingest.Verify(i => i.ExecuteAsync(It.IsAny<ExternalMatchReport>(), It.IsAny<CancellationToken>()), Times.Once);
        _recalculate.Verify(r => r.ExecuteAsync(2023, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Import_NothingNew_DoesNotRecalculate()
    {
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023, Fixture("1", 2023));
        _existing.Add("1");

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2023, ApiFootballCompetitions.PremierLeague));

        summary.Imported.Should().Be(0);
        _recalculate.Verify(r => r.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Import_FetchesDetailsInChunksOfTheBatchSize()
    {
        var fixtures = Enumerable.Range(1, 45).Select(i => Fixture(i.ToString(), 2023)).ToArray();
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023, fixtures);

        await Importer().ImportAsync(new SeasonImportRequest(2023, ApiFootballCompetitions.PremierLeague));

        _provider.Verify(p => p.GetMatchReportsAsync(It.Is<IReadOnlyList<ExternalFixture>>(f => f.Count == 20), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _provider.Verify(p => p.GetMatchReportsAsync(It.Is<IReadOnlyList<ExternalFixture>>(f => f.Count == 5), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Import_RespectsTheMatchLimitAcrossCompetitions()
    {
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023, Fixture("1", 2023), Fixture("2", 2023), Fixture("3", 2023));
        ListFixtures(ApiFootballCompetitions.LaLiga, 2023, Fixture("4", 2023));

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2023, MaxMatches: 2));

        summary.Stop.Should().Be(SeasonImportStop.LimitReached);
        summary.Imported.Should().Be(2);
        summary.Competitions.Should().ContainSingle().Which.Pending.Should().Be(1);
    }

    [Fact]
    public async Task Import_NationalTeamTournament_KeepsOnlyMatchesOfTheRequestedSeason()
    {
        // Copa de 2026 (API season 2026) pertence a 2025/26; uma partida de outra temporada não deve entrar
        ListFixtures(ApiFootballCompetitions.WorldCup, 2026, Fixture("10", 2025), Fixture("11", 2026));

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2025, ApiFootballCompetitions.WorldCup));

        summary.Competitions.Single().Finished.Should().Be(1);
        _ingest.Verify(i => i.ExecuteAsync(It.Is<ExternalMatchReport>(r => r.Fixture.ExternalId == "10"), It.IsAny<CancellationToken>()), Times.Once);
        _recalculate.Verify(r => r.ExecuteAsync(2025, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Import_CountsMatchesWithoutPlayerDataAndMissingBatchItems()
    {
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023, Fixture("pl-1", 2023));
        ListFixtures(ApiFootballCompetitions.FaCup, 2023, Fixture("1", 2023), Fixture("2", 2023), Fixture("3", 2023));
        _provider
            .Setup(p => p.GetMatchReportsAsync(It.IsAny<IReadOnlyList<ExternalFixture>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ExternalFixture> fixtures, CancellationToken _) =>
            [
                new ExternalMatchReport(fixtures[0], [Performance()]),
                new ExternalMatchReport(fixtures[1], []),
            ]);

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2023, ApiFootballCompetitions.FaCup));

        var line = summary.Competitions.Single();
        line.Imported.Should().Be(1);
        line.WithoutPlayerData.Should().Be(1);
        line.NotReturned.Should().Be(1);
        line.Pending.Should().Be(1);
    }

    [Fact]
    public async Task Import_QuotaExhausted_StopsGracefullyAndKeepsWhatWasImported()
    {
        var fixtures = Enumerable.Range(1, 30).Select(i => Fixture(i.ToString(), 2023)).ToArray();
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023, fixtures);
        _provider
            .SetupSequence(p => p.GetMatchReportsAsync(It.IsAny<IReadOnlyList<ExternalFixture>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixtures.Take(20).Select(f => new ExternalMatchReport(f, [Performance()])).ToList())
            .ThrowsAsync(new ApiFootballException(ApiFootballErrorKind.DailyQuotaExceeded, "quota"));

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2023));

        summary.Stop.Should().Be(SeasonImportStop.QuotaExhausted);
        summary.Imported.Should().Be(20);
        summary.Competitions.Should().ContainSingle().Which.Pending.Should().Be(10);
        _recalculate.Verify(r => r.ExecuteAsync(2023, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Import_SeasonOutsideThePlan_Stops()
    {
        _provider
            .Setup(p => p.GetFixturesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiFootballException(ApiFootballErrorKind.SeasonNotAvailable, "plan"));

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2026));

        summary.Stop.Should().Be(SeasonImportStop.SeasonNotInPlan);
        summary.Competitions.Should().ContainSingle();
        _provider.Verify(p => p.GetFixturesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Import_IngestionFailure_StopsTheRun()
    {
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023, Fixture("1", 2023), Fixture("2", 2023));
        _ingest
            .Setup(i => i.ExecuteAsync(It.IsAny<ExternalMatchReport>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db"));

        var act = () => Importer().ImportAsync(new SeasonImportRequest(2023, ApiFootballCompetitions.PremierLeague));

        await act.Should().ThrowAsync<InvalidOperationException>();
        _ingest.Verify(i => i.ExecuteAsync(It.IsAny<ExternalMatchReport>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Import_DomesticCup_SkipsMatchesWithoutTopLeagueTeam()
    {
        // FA Cup: fases qualificatórias entre clubes semiamadores não têm dados de jogador na fonte
        ListFixtures(ApiFootballCompetitions.PremierLeague, 2023, Fixture("pl-1", 2023));
        ListFixtures(ApiFootballCompetitions.FaCup, 2023,
            CupFixture("cup-amateur", homeId: "9001", awayId: "9002"),
            CupFixture("cup-city", homeId: "9003", awayId: "50"));

        var summary = await Importer().ImportAsync(new SeasonImportRequest(2023, ApiFootballCompetitions.FaCup));

        var line = summary.Competitions.Should().ContainSingle().Subject;
        line.Listed.Should().Be(2);
        line.Finished.Should().Be(1);
        _ingest.Verify(i => i.ExecuteAsync(It.Is<ExternalMatchReport>(r => r.Fixture.ExternalId == "cup-city"), It.IsAny<CancellationToken>()), Times.Once);
        _ingest.Verify(i => i.ExecuteAsync(It.IsAny<ExternalMatchReport>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ExternalFixture CupFixture(string id, string homeId, string awayId) =>
        Fixture(id, 2023) with
        {
            Competition = new ExternalCompetition("45", "FA Cup", "England", CompetitionTier.DomesticCup, 2023),
            HomeTeam = new ExternalTeam(homeId, "Home", string.Empty),
            AwayTeam = new ExternalTeam(awayId, "Away", string.Empty),
            RoundPhase = "3rd Round",
            IsKnockout = true,
        };

    private SeasonImporter Importer() =>
        new(_provider.Object, _matches.Object, _ingest.Object, _recalculate.Object, Mock.Of<IUnitOfWork>(), NullLogger<SeasonImporter>.Instance);

    private void ListFixtures(int leagueId, int apiSeason, params ExternalFixture[] fixtures) =>
        _provider
            .Setup(p => p.GetFixturesAsync(leagueId.ToString(), apiSeason, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixtures);

    private static ExternalFixture Fixture(string id, int footballSeason, bool finished = true) =>
        new(
            id,
            new ExternalCompetition("39", "Premier League", "England", CompetitionTier.TopLeague, footballSeason),
            new ExternalTeam("50", "Manchester City", string.Empty),
            new ExternalTeam("33", "Manchester United", string.Empty),
            "Regular Season - 1",
            IsKnockout: false,
            new DateTime(2024, 1, 1, 15, 0, 0, DateTimeKind.Utc),
            finished,
            HomeScore: finished ? 1 : null,
            AwayScore: finished ? 0 : null);

    private static ExternalPlayerPerformance Performance() =>
        new(
            new ExternalPlayer("44", "Rodri", string.Empty),
            "50",
            new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90 });
}
