namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using TheRealBest.Domain.Enums;

/// <summary>
/// IDs de competição da API-Football cobertos pelo projeto e seu nível para o W_torneio.
/// </summary>
public static class ApiFootballCompetitions
{
    public const int WorldCup = 1;
    public const int ChampionsLeague = 2;
    public const int EuroChampionship = 4;
    public const int CopaAmerica = 9;
    public const int PremierLeague = 39;
    public const int LaLiga = 140;
    public const int SerieA = 135;
    public const int Bundesliga = 78;
    public const int Ligue1 = 61;
    public const int FaCup = 45;
    public const int CopaDelRey = 143;
    public const int CoppaItalia = 137;
    public const int DfbPokal = 81;
    public const int CoupeDeFrance = 66;

    public static CompetitionTier TierFor(int leagueId) => leagueId switch
    {
        WorldCup => CompetitionTier.WorldCup,
        ChampionsLeague => CompetitionTier.UclKnockout,
        EuroChampionship or CopaAmerica => CompetitionTier.InternationalContinental,
        PremierLeague or LaLiga or SerieA or Bundesliga or Ligue1 => CompetitionTier.TopLeague,
        FaCup or CopaDelRey or CoppaItalia or DfbPokal or CoupeDeFrance => CompetitionTier.DomesticCup,
        _ => CompetitionTier.OtherLeague,
    };

    /// <summary>
    /// Temporada do calendário do futebol europeu (agosto a julho) à qual a partida pertence.
    /// Ligas usam o ano de início na própria API; torneios de seleções usam o ano do torneio, então a Euro e a
    /// Copa América de junho/julho de 2024 (API season 2024) pertencem à temporada 2023/24, a do ciclo da Bola de Ouro 2024.
    /// </summary>
    public static int FootballSeasonOf(int leagueId, int apiSeason, DateTimeOffset kickoff)
    {
        var tier = TierFor(leagueId);
        var isNationalTeamTournament = tier is CompetitionTier.WorldCup or CompetitionTier.InternationalContinental;
        if (!isNationalTeamTournament)
        {
            return apiSeason;
        }

        var date = kickoff.UtcDateTime;
        return date.Month >= 8 ? date.Year : date.Year - 1;
    }

    /// <summary>
    /// Rodadas de pontos corridos e fases de grupos/liga não são mata-mata ("Regular Season - 5", "Group A - 2", "League Stage - 3").
    /// </summary>
    public static bool IsKnockoutRound(string round) =>
        !(round.StartsWith("Regular Season", StringComparison.OrdinalIgnoreCase)
          || round.StartsWith("Group", StringComparison.OrdinalIgnoreCase)
          || round.StartsWith("League Stage", StringComparison.OrdinalIgnoreCase));
}
