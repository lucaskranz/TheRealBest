namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Globalization;
using System.Text.Json;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// Converte as respostas da API-Football nos registros de importação do domínio.
/// </summary>
/// <remarks>
/// A API-Football não fornece duelos aéreos separados, recuperações, passes progressivos, xG, xA, grandes chances
/// nem erros que levaram a gol: esses campos ficam em zero até o enriquecimento por outra fonte (FBref).
/// </remarks>
public static class ApiFootballMapper
{
    private static readonly HashSet<string> FinishedStatuses = ["FT", "AET", "PEN"];

    /// <summary>Ações cujas estatísticas a API-Football não fornece: nesta fonte elas sempre valem zero.</summary>
    public static IReadOnlySet<ActionType> UnavailableActions { get; } = new HashSet<ActionType>
    {
        ActionType.ExpectedAssists,
        ActionType.BigChanceCreated,
        ActionType.ExpectedGoalsOverperformance,
        ActionType.ProgressivePass,
        ActionType.AerialDuelWon,
        ActionType.BallRecovery,
        ActionType.ErrorLeadingToGoal,
        ActionType.BigChanceMissed,
    };

    public static ExternalFixture ToFixture(FixtureItem item) =>
        new(
            ExternalId: Id(item.Fixture.Id),
            Competition: new ExternalCompetition(
                Id(item.League.Id),
                item.League.Name,
                item.League.Country ?? string.Empty,
                ApiFootballCompetitions.TierFor(item.League.Id),
                ApiFootballCompetitions.FootballSeasonOf(item.League.Id, item.League.Season, item.Fixture.Date),
                ApiFootballCompetitions.LocalizedNames(item.League.Id)),
            HomeTeam: ToTeam(item.Teams.Home),
            AwayTeam: ToTeam(item.Teams.Away),
            RoundPhase: item.League.Round,
            IsKnockout: ApiFootballCompetitions.IsKnockoutRound(item.League.Round),
            KickoffUtc: item.Fixture.Date.UtcDateTime,
            IsFinished: FinishedStatuses.Contains(item.Fixture.Status.Short),
            HomeScore: item.Goals.Home,
            AwayScore: item.Goals.Away);

    public static ExternalMatchReport ToMatchReport(
        ExternalFixture fixture,
        IReadOnlyList<FixturePlayersItem> players,
        IReadOnlyList<EventItem> events,
        IReadOnlyList<LineupItem> lineups)
    {
        var homeTeamId = int.Parse(fixture.HomeTeam.ExternalId, CultureInfo.InvariantCulture);
        var awayTeamId = int.Parse(fixture.AwayTeam.ExternalId, CultureInfo.InvariantCulture);

        var starters = lineups.SelectMany(l => l.StartXI ?? []).Select(s => s.Player.Id).ToHashSet();
        var timeline = MatchTimeline.Build(events, starters, homeTeamId, awayTeamId, fixture.HomeScore, fixture.AwayScore);

        var appearances = players
            .SelectMany(team => team.Players.Select(entry => (Team: team.Team, entry.Player, Stats: entry.Statistics.FirstOrDefault())))
            .Where(a => a.Stats?.Games.Minutes is > 0)
            // Jogadores sem ID (clubes amadores nas copas) não podem ser identificados entre partidas: ficam de fora
            .Where(a => a.Player.Id > 0)
            .DistinctBy(a => a.Player.Id)
            .ToList();

        var positions = PositionResolver.Resolve(
            lineups,
            timeline.Substitutions,
            appearances.ToDictionary(a => a.Player.Id, a => a.Stats!.Games.Position));

        var performances = appearances
            .Select(a => new ExternalPlayerPerformance(
                new ExternalPlayer(Id(a.Player.Id), a.Player.Name, a.Player.Photo ?? string.Empty),
                Id(a.Team.Id),
                ToStatLine(a.Stats!, positions[a.Player.Id], timeline.GoalsConcededWhileOnPitch(a.Player.Id, a.Team.Id), timeline.OwnGoalsBy(a.Player.Id))))
            .ToList();

        return new ExternalMatchReport(fixture, performances);
    }

    private static PlayerStatLine ToStatLine(PlayerStatistics s, Domain.Enums.PlayerPosition position, int goalsConceded, int ownGoals)
    {
        var passesTotal = s.Passes.Total ?? 0;

        return new PlayerStatLine
        {
            Position = position,
            MinutesPlayed = s.Games.Minutes ?? 0,

            Goals = s.Goals.Total ?? 0,
            PenaltiesScored = s.Penalty.Scored ?? 0,
            Assists = s.Goals.Assists ?? 0,
            ShotsTotal = s.Shots.Total ?? 0,
            ShotsOnTarget = s.Shots.On ?? 0,
            KeyPasses = s.Passes.Key ?? 0,

            PassesTotal = passesTotal,
            PassesAccurate = Math.Min(passesTotal, AccuratePasses(s.Passes.Accuracy)),

            TacklesTotal = s.Tackles.Total ?? 0,
            Interceptions = s.Tackles.Interceptions ?? 0,
            Blocks = s.Tackles.Blocks ?? 0,
            DuelsTotal = s.Duels.Total ?? 0,
            DuelsWon = s.Duels.Won ?? 0,
            DribblesAttempted = s.Dribbles.Attempts ?? 0,
            DribblesSuccess = s.Dribbles.Success ?? 0,
            DribbledPast = s.Dribbles.Past ?? 0,
            OwnGoals = ownGoals,

            Saves = s.Goals.Saves ?? 0,
            GoalsConceded = goalsConceded,
            CleanSheet = goalsConceded == 0,
            PenaltiesSaved = s.Penalty.Saved ?? 0,
            FoulsCommitted = s.Fouls.Committed ?? 0,
            FoulsDrawn = s.Fouls.Drawn ?? 0,
            YellowCards = s.Cards.Yellow ?? 0,
            RedCards = s.Cards.Red ?? 0,
            PenaltiesWon = s.Penalty.Won ?? 0,
            PenaltiesCommitted = s.Penalty.Committed ?? 0,
            PenaltiesMissed = s.Penalty.Missed ?? 0,
            Offsides = s.Offsides ?? 0,
        };
    }

    /// <summary>"accuracy" é a quantidade de passes certos e costuma vir como string ("61").</summary>
    private static int AccuratePasses(JsonElement accuracy) => accuracy.ValueKind switch
    {
        JsonValueKind.Number when accuracy.TryGetInt32(out var number) => number,
        JsonValueKind.String when int.TryParse(accuracy.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) => parsed,
        _ => 0,
    };

    private static ExternalTeam ToTeam(TeamInfo team) => new(Id(team.Id), team.Name, team.Logo ?? string.Empty);

    private static string Id(int id) => id.ToString(CultureInfo.InvariantCulture);
}
