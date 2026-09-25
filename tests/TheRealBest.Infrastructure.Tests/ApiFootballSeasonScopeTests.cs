namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using static TheRealBest.Infrastructure.ExternalApis.ApiFootball.ApiFootballCompetitions;

public class ApiFootballSeasonScopeTests
{
    [Theory]
    [InlineData(2022)]
    [InlineData(2023)]
    [InlineData(2024)]
    [InlineData(2025)]
    [InlineData(2026)]
    public void For_EverySeason_IncludesTheClubCompetitionsWithTheStartYear(int season)
    {
        var scope = ApiFootballSeasonScope.For(season);

        int[] clubs = [PremierLeague, LaLiga, SerieA, Bundesliga, Ligue1, ChampionsLeague, FaCup, CopaDelRey, CoppaItalia, DfbPokal, CoupeDeFrance];
        scope.Where(e => clubs.Contains(e.LeagueId)).Should().HaveCount(clubs.Length).And.OnlyContain(e => e.ApiSeason == season);
    }

    [Theory]
    [InlineData(2022, new[] { WorldCup }, new[] { 2022 })]
    [InlineData(2023, new[] { EuroChampionship, CopaAmerica }, new[] { 2024, 2024 })]
    [InlineData(2024, new int[0], new int[0])]
    [InlineData(2025, new[] { WorldCup }, new[] { 2026 })]
    [InlineData(2026, new int[0], new int[0])]
    public void For_NationalTeamTournaments_FallInTheSeasonTheyWerePlayed(int season, int[] leagues, int[] apiSeasons)
    {
        var tournaments = ApiFootballSeasonScope.For(season)
            .Where(e => TierFor(e.LeagueId) is Domain.Enums.CompetitionTier.WorldCup or Domain.Enums.CompetitionTier.InternationalContinental)
            .ToList();

        tournaments.Select(e => e.LeagueId).Should().Equal(leagues);
        tournaments.Select(e => e.ApiSeason).Should().Equal(apiSeasons);
    }

    [Theory]
    [InlineData(2021)]
    [InlineData(2027)]
    public void For_UnsupportedSeason_Throws(int season)
    {
        var act = () => ApiFootballSeasonScope.For(season);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
