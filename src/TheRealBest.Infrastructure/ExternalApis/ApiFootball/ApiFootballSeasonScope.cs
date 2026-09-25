namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using static TheRealBest.Infrastructure.ExternalApis.ApiFootball.ApiFootballCompetitions;

/// <summary>Uma competição a importar, com a temporada no formato da API-Football.</summary>
public sealed record SeasonScopeEntry(int LeagueId, int ApiSeason);

/// <summary>
/// Competições cobertas em cada temporada do futebol europeu (seção "Fase 5" do CONTEXT.md).
/// Clubes usam o ano de início como temporada na API. Torneios de seleções usam o ano do torneio e entram na
/// temporada em que foram disputados: Copa de 2022 (nov./dez.) em 2022/23; Euro e Copa América 2024 (jun./jul.) em 2023/24;
/// Copa de 2026 (jun./jul.) em 2025/26.
/// </summary>
public static class ApiFootballSeasonScope
{
    public const int FirstSeason = 2022;

    /// <summary>Temporada em andamento (2026/27). Atualizar a cada agosto.</summary>
    public const int CurrentSeason = 2026;

    private static readonly int[] ClubCompetitions =
    [
        PremierLeague, LaLiga, SerieA, Bundesliga, Ligue1,
        ChampionsLeague,
        FaCup, CopaDelRey, CoppaItalia, DfbPokal, CoupeDeFrance,
    ];

    private static readonly IReadOnlyDictionary<int, SeasonScopeEntry[]> NationalTeamTournaments = new Dictionary<int, SeasonScopeEntry[]>
    {
        [2022] = [new(WorldCup, 2022)],
        [2023] = [new(EuroChampionship, 2024), new(CopaAmerica, 2024)],
        [2025] = [new(WorldCup, 2026)],
    };

    private static readonly IReadOnlyDictionary<int, int> TopLeagueByCup = new Dictionary<int, int>
    {
        [FaCup] = PremierLeague,
        [CopaDelRey] = LaLiga,
        [CoppaItalia] = SerieA,
        [DfbPokal] = Bundesliga,
        [CoupeDeFrance] = Ligue1,
    };

    /// <summary>
    /// Primeira divisão do país de uma copa nacional, ou nulo se não for copa. Das copas só entram as partidas com ao menos
    /// um time dessa liga: as fases preliminares e regionais, entre amadores, não têm estatísticas de jogador na fonte.
    /// </summary>
    public static int? TopLeagueOf(int cupId) => TopLeagueByCup.TryGetValue(cupId, out var league) ? league : null;

    public static bool IsSupported(int season) => season is >= FirstSeason and <= CurrentSeason;

    /// <param name="season">Ano de início da temporada (2023 = 2023/24).</param>
    public static IReadOnlyList<SeasonScopeEntry> For(int season)
    {
        if (!IsSupported(season))
        {
            throw new ArgumentOutOfRangeException(nameof(season), season, $"Supported seasons: {FirstSeason} to {CurrentSeason}.");
        }

        return ClubCompetitions
            .Select(league => new SeasonScopeEntry(league, season))
            .Concat(NationalTeamTournaments.GetValueOrDefault(season, []))
            .ToList();
    }
}
