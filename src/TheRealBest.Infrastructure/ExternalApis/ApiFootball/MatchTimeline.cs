namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <summary>
/// Reconstrói a partida a partir dos eventos: substituições, expulsões, gols (sem disputa de pênaltis)
/// e quem estava em campo em cada gol. A API só informa gols sofridos para goleiros; para os demais
/// jogadores eles são derivados daqui.
/// </summary>
public sealed class MatchTimeline
{
    private const string ShootoutComment = "Penalty Shootout";

    private readonly List<(decimal Minute, int BenefitingTeamId)> _goals;
    private readonly Dictionary<int, decimal> _enteredAt;
    private readonly Dictionary<int, decimal> _leftAt;
    private readonly Dictionary<int, int> _ownGoals;

    private MatchTimeline(
        List<(decimal, int)> goals,
        Dictionary<int, decimal> enteredAt,
        Dictionary<int, decimal> leftAt,
        Dictionary<int, int> ownGoals,
        IReadOnlyList<Substitution> substitutions,
        bool matchesFinalScore)
    {
        _goals = goals;
        _enteredAt = enteredAt;
        _leftAt = leftAt;
        _ownGoals = ownGoals;
        Substitutions = substitutions;
        MatchesFinalScore = matchesFinalScore;
    }

    public sealed record Substitution(int TeamId, int PlayerInId, int PlayerOutId, decimal Minute);

    /// <summary>Substituições em ordem cronológica.</summary>
    public IReadOnlyList<Substitution> Substitutions { get; }

    /// <summary>Falso se os gols dos eventos não reproduzem o placar final (eventos incompletos na fonte).</summary>
    public bool MatchesFinalScore { get; }

    /// <param name="starters">Titulares das duas equipes, usados para validar a ordem sai/entra das substituições.</param>
    public static MatchTimeline Build(
        IEnumerable<EventItem> events,
        IReadOnlySet<int> starters,
        int homeTeamId,
        int awayTeamId,
        int? homeScore,
        int? awayScore)
    {
        var ordered = events
            .Where(e => !string.Equals(e.Comments, ShootoutComment, StringComparison.OrdinalIgnoreCase))
            .OrderBy(MinuteOf)
            .ToList();

        var substitutions = new List<Substitution>();
        var enteredAt = new Dictionary<int, decimal>();
        var leftAt = new Dictionary<int, decimal>();
        var ownGoals = new Dictionary<int, int>();
        var goalEvents = new List<EventItem>();

        foreach (var e in ordered)
        {
            switch (e.Type.ToLowerInvariant())
            {
                case "subst" when e.Player.Id is { } outId && e.Assist.Id is { } inId:
                    // Convenção da API: player = sai, assist = entra. Se quem "entra" já estava em campo, o par veio invertido.
                    if (starters.Contains(inId) || enteredAt.ContainsKey(inId))
                    {
                        (inId, outId) = (outId, inId);
                    }

                    substitutions.Add(new Substitution(e.Team.Id, inId, outId, MinuteOf(e)));
                    enteredAt[inId] = MinuteOf(e);
                    leftAt.TryAdd(outId, MinuteOf(e));
                    break;

                case "card" when IsSendingOff(e.Detail) && e.Player.Id is { } sentOffId:
                    leftAt.TryAdd(sentOffId, MinuteOf(e));
                    break;

                case "goal" when !IsDetail(e, "Missed Penalty"):
                    goalEvents.Add(e);
                    if (IsDetail(e, "Own Goal") && e.Player.Id is { } ownGoalScorer)
                    {
                        ownGoals[ownGoalScorer] = ownGoals.GetValueOrDefault(ownGoalScorer) + 1;
                    }

                    break;

                case "var" when e.Detail?.Contains("cancelled", StringComparison.OrdinalIgnoreCase) == true
                             || e.Detail?.Contains("disallowed", StringComparison.OrdinalIgnoreCase) == true:
                    // Gol anulado: remove o último gol daquele time registrado até o minuto do VAR
                    var cancelled = goalEvents.LastOrDefault(g => g.Team.Id == e.Team.Id && MinuteOf(g) <= MinuteOf(e));
                    if (cancelled is not null)
                    {
                        goalEvents.Remove(cancelled);
                    }

                    break;
            }
        }

        var (goals, matchesScore) = ResolveBenefitingTeams(goalEvents, homeTeamId, awayTeamId, homeScore, awayScore);
        return new MatchTimeline(goals, enteredAt, leftAt, ownGoals, substitutions, matchesScore);
    }

    public int GoalsConcededWhileOnPitch(int playerId, int teamId)
    {
        var from = _enteredAt.GetValueOrDefault(playerId, 0m);
        var until = _leftAt.GetValueOrDefault(playerId, decimal.MaxValue);
        return _goals.Count(g => g.BenefitingTeamId != teamId && g.Minute >= from && g.Minute <= until);
    }

    public int OwnGoalsBy(int playerId) => _ownGoals.GetValueOrDefault(playerId);

    /// <summary>
    /// Minuto ordenável: acréscimos entram como fração (45+2 → 45.02), para ficarem antes do minuto 46.
    /// </summary>
    private static decimal MinuteOf(EventItem e) => e.Time.Elapsed + (e.Time.Extra ?? 0) / 100m;

    private static bool IsDetail(EventItem e, string detail) =>
        string.Equals(e.Detail, detail, StringComparison.OrdinalIgnoreCase);

    private static bool IsSendingOff(string? detail) =>
        detail is not null
        && (detail.Contains("Red Card", StringComparison.OrdinalIgnoreCase)
            || detail.Contains("Second Yellow", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Em gols contra, a fonte pode atribuir o evento ao time de quem marcou ou ao time beneficiado.
    /// Testa as duas leituras e fica com a que reproduz o placar final.
    /// </summary>
    private static (List<(decimal, int)> Goals, bool MatchesScore) ResolveBenefitingTeams(
        List<EventItem> goalEvents, int homeTeamId, int awayTeamId, int? homeScore, int? awayScore)
    {
        int Opponent(int teamId) => teamId == homeTeamId ? awayTeamId : homeTeamId;

        List<(decimal, int)> Read(bool ownGoalEventBelongsToScorerTeam) => goalEvents
            .Select(g => (MinuteOf(g), IsDetail(g, "Own Goal") && ownGoalEventBelongsToScorerTeam ? Opponent(g.Team.Id) : g.Team.Id))
            .ToList();

        bool Matches(List<(decimal, int Team)> goals) =>
            homeScore is not null && awayScore is not null
            && goals.Count(g => g.Team == homeTeamId) == homeScore
            && goals.Count(g => g.Team == awayTeamId) == awayScore;

        var scorerTeamReading = Read(ownGoalEventBelongsToScorerTeam: true);
        if (Matches(scorerTeamReading))
        {
            return (scorerTeamReading, true);
        }

        var beneficiaryReading = Read(ownGoalEventBelongsToScorerTeam: false);
        return Matches(beneficiaryReading) ? (beneficiaryReading, true) : (scorerTeamReading, false);
    }
}
