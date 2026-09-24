namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Infrastructure.Tests.Support;
using static TheRealBest.Infrastructure.Tests.Support.ApiFootballFixtures;

/// <summary>
/// Mapeamento de respostas reais da API-Football (ver Support/ApiFootballFixtures).
/// </summary>
public class ApiFootballMapperTests
{
    [Fact]
    public void ToFixture_UclFinal_MapsCompetitionTeamsAndScore()
    {
        var fixture = Fixture(UclFinal2023);

        fixture.ExternalId.Should().Be("1027909");
        fixture.Competition.Should().BeEquivalentTo(new ExternalCompetition(
            "2", "UEFA Champions League", "World", CompetitionTier.UclKnockout, 2022,
            new Dictionary<string, string>
            {
                ["pt-BR"] = "Liga dos Campeões da UEFA",
                ["en"] = "UEFA Champions League",
                ["es"] = "Liga de Campeones de la UEFA",
            }));
        fixture.HomeTeam.Should().Match<ExternalTeam>(t => t.ExternalId == "50" && t.Name == "Manchester City");
        fixture.AwayTeam.ExternalId.Should().Be("505");
        fixture.RoundPhase.Should().Be("Final");
        fixture.IsKnockout.Should().BeTrue();
        fixture.IsFinished.Should().BeTrue();
        fixture.KickoffUtc.Should().Be(new DateTime(2023, 6, 10, 19, 0, 0, DateTimeKind.Utc));
        fixture.KickoffUtc.Kind.Should().Be(DateTimeKind.Utc);
        (fixture.HomeScore, fixture.AwayScore).Should().Be((1, 0));
    }

    [Theory]
    [InlineData(ApiFootballCompetitions.EuroChampionship, 2024, "2024-07-14", 2023)] // Final da Euro 2024 → temporada 2023/24
    [InlineData(ApiFootballCompetitions.CopaAmerica, 2024, "2024-07-15", 2023)]
    [InlineData(ApiFootballCompetitions.WorldCup, 2022, "2022-12-18", 2022)]       // Copa do Catar → temporada 2022/23
    [InlineData(ApiFootballCompetitions.PremierLeague, 2023, "2024-05-19", 2023)]  // Ligas: season da API
    public void FootballSeasonOf_MapsNationalTeamTournamentsToClubSeason(int leagueId, int apiSeason, string kickoff, int expected)
    {
        ApiFootballCompetitions.FootballSeasonOf(leagueId, apiSeason, DateTimeOffset.Parse(kickoff + "T19:00:00Z"))
            .Should().Be(expected);
    }

    [Fact]
    public void ToFixture_MatchDecidedOnPenalties_IsFinishedWithScoreAfterExtraTime()
    {
        var fixture = Fixture(UclQuarterFinal2024);

        fixture.IsFinished.Should().BeTrue();
        (fixture.HomeScore, fixture.AwayScore).Should().Be((1, 1), "the shootout does not count as goals");
    }

    [Fact]
    public void ToMatchReport_IncludesOnlyPlayersWhoPlayed()
    {
        var report = Report(UclFinal2023);

        // City: 11 titulares + Foden e Walker; Inter: 11 titulares + 5 substituições
        report.Performances.Should().HaveCount(29);
        report.Performances.Should().OnlyContain(p => p.Stats.MinutesPlayed > 0);
        report.Performances.Count(p => p.TeamExternalId == "50").Should().Be(13);
    }

    [Fact]
    public void ToMatchReport_Rodri_MapsRawStatistics()
    {
        var rodri = Player(Report(UclFinal2023), "44").Stats;

        rodri.Should().BeEquivalentTo(new PlayerStatLine
        {
            Position = PlayerPosition.CDM,
            MinutesPlayed = 90,
            Goals = 1,
            ShotsTotal = 1,
            ShotsOnTarget = 1,
            KeyPasses = 2,
            PassesTotal = 66,
            PassesAccurate = 61, // "accuracy": "61" é a quantidade de passes certos
            Interceptions = 3,
            Blocks = 1,
            DuelsTotal = 9,
            DuelsWon = 4,
            DribblesAttempted = 1,
            DribblesSuccess = 1,
            DribbledPast = 1,
            GoalsConceded = 0,
            CleanSheet = true,
        });
    }

    [Fact]
    public void ToMatchReport_Goalkeepers_ConcededFromTimelineMatchesApiValue()
    {
        var report = Report(UclFinal2023);

        var onana = Player(report, "526").Stats;
        onana.Position.Should().Be(PlayerPosition.GK);
        onana.GoalsConceded.Should().Be(1);
        onana.CleanSheet.Should().BeFalse();
        onana.Saves.Should().Be(3);
        onana.YellowCards.Should().Be(1);

        var ederson = Player(report, "617").Stats;
        ederson.GoalsConceded.Should().Be(0);
        ederson.CleanSheet.Should().BeTrue();
    }

    [Theory]
    [InlineData("30558", 1)] // Barella: 90 minutos, em campo no gol de Rodri (68')
    [InlineData("907", 1)]   // Lukaku: entrou aos 57'
    [InlineData("790", 0)]   // Džeko: saiu aos 57', antes do gol
    [InlineData("30422", 0)] // Gosens: entrou aos 76', depois do gol
    public void ToMatchReport_OutfieldGoalsConceded_DependOnTimeOnPitch(string playerId, int expectedConceded)
    {
        var stats = Player(Report(UclFinal2023), playerId).Stats;

        stats.GoalsConceded.Should().Be(expectedConceded);
        stats.CleanSheet.Should().Be(expectedConceded == 0);
    }

    [Theory]
    [InlineData("617", 1)]  // Ederson: 120 minutos, sofreu só o gol de Rodrygo (a disputa de pênaltis não conta)
    [InlineData("47400", 1)] // Lunin
    [InlineData("19187", 1)] // Grealish: saiu aos 72', depois do gol de Rodrygo (12')
    [InlineData("1422", 0)]  // Doku: entrou aos 72'
    [InlineData("6009", 0)]  // Álvarez: entrou aos 90'
    [InlineData("2285", 1)]  // Rüdiger: 120 minutos, em campo no gol de De Bruyne (76')
    public void ToMatchReport_ExtraTimeAndShootout_ConcededIgnoresShootout(string playerId, int expectedConceded)
    {
        Player(Report(UclQuarterFinal2024), playerId).Stats.GoalsConceded.Should().Be(expectedConceded);
    }

    [Fact]
    public void ToMatchReport_Shootout_DoesNotCountAsPenaltyGoals()
    {
        var report = Report(UclQuarterFinal2024);

        report.Performances.Sum(p => p.Stats.Goals).Should().Be(2);
        report.Performances.Should().OnlyContain(p => p.Stats.PenaltiesScored == 0);
        Player(report, "2285").Stats.MinutesPlayed.Should().Be(120);
    }

    private static ExternalPlayerPerformance Player(ExternalMatchReport report, string externalId) =>
        report.Performances.Single(p => p.Player.ExternalId == externalId);
}
