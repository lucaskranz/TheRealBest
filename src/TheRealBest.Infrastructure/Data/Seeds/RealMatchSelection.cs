namespace TheRealBest.Infrastructure.Data.Seeds;

using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <summary>
/// Partida selecionada para o seed, identificada pelos IDs da API-Football.
/// Placar, escalações e estatísticas vêm da API na importação; aqui ficam só os identificadores.
/// </summary>
/// <param name="ApiSeason">Parâmetro "season" da API (Euro e Copa América 2024 usam 2024).</param>
public sealed record SeedMatch(int LeagueId, int ApiSeason, int FixtureId, string Description);

/// <summary>
/// Amostra de 30 partidas marcantes do ciclo da Bola de Ouro 2024 (temporada 2023/24, Euro e Copa América 2024).
/// IDs conferidos contra as listagens da API (data, times e placar).
/// </summary>
public static class RealMatchSelection
{
    private const int Ucl = ApiFootballCompetitions.ChampionsLeague;
    private const int Epl = ApiFootballCompetitions.PremierLeague;
    private const int Liga = ApiFootballCompetitions.LaLiga;
    private const int Bun = ApiFootballCompetitions.Bundesliga;
    private const int Euro = ApiFootballCompetitions.EuroChampionship;
    private const int Copa = ApiFootballCompetitions.CopaAmerica;

    public static IReadOnlyList<SeedMatch> Season2023 { get; } =
    [
        // UEFA Champions League 2023/24
        new(Ucl, 2023, 1126153, "Real Madrid 1-0 Union Berlin (fase de grupos)"),
        new(Ucl, 2023, 1126166, "Napoli 2-3 Real Madrid (fase de grupos)"),
        new(Ucl, 2023, 1126197, "Manchester City 3-0 Young Boys (fase de grupos)"),
        new(Ucl, 2023, 1126220, "Real Madrid 4-2 Napoli (fase de grupos)"),
        new(Ucl, 2023, 1149510, "Copenhagen 1-3 Manchester City (oitavas)"),
        new(Ucl, 2023, 1149513, "Real Madrid 1-1 RB Leipzig (oitavas)"),
        new(Ucl, 2023, 1184779, "Real Madrid 3-3 Manchester City (quartas, ida)"),
        new(Ucl, 2023, 1184780, "Manchester City 1-1 Real Madrid (quartas, volta, pênaltis)"),
        new(Ucl, 2023, 1196543, "Bayern 2-2 Real Madrid (semifinal, ida)"),
        new(Ucl, 2023, 1196544, "Real Madrid 2-1 Bayern (semifinal, volta)"),
        new(Ucl, 2023, 1199389, "Borussia Dortmund 0-2 Real Madrid (final)"),

        // Premier League 2023/24
        new(Epl, 2023, 1035075, "Manchester City 5-1 Fulham"),
        new(Epl, 2023, 1035409, "Manchester City 2-0 Everton"),
        new(Epl, 2023, 1035419, "Manchester City 1-1 Chelsea"),
        new(Epl, 2023, 1035439, "Manchester City 3-1 Manchester United"),
        new(Epl, 2023, 1035483, "Manchester City 4-1 Aston Villa"),
        new(Epl, 2023, 1035512, "Tottenham 0-2 Manchester City"),
        new(Epl, 2023, 1035532, "Manchester City 5-1 Wolves"),
        new(Epl, 2023, 1035552, "Manchester City 3-1 West Ham (última rodada)"),

        // La Liga 2023/24
        new(Liga, 2023, 1038054, "Barcelona 1-2 Real Madrid"),
        new(Liga, 2023, 1038190, "Real Madrid 4-0 Girona"),
        new(Liga, 2023, 1038234, "Osasuna 2-4 Real Madrid"),
        new(Liga, 2023, 1038267, "Real Madrid 3-2 Barcelona"),

        // Bundesliga 2023/24
        new(Bun, 2023, 1049135, "Bayer Leverkusen 5-0 Werder Bremen (título)"),

        // Euro 2024
        new(Euro, 2024, 1145521, "Espanha 1-0 Itália (fase de grupos)"),
        new(Euro, 2024, 1215075, "Espanha 4-1 Geórgia (oitavas)"),
        new(Euro, 2024, 1219688, "Espanha 2-1 Alemanha (quartas, prorrogação)"),
        new(Euro, 2024, 1225853, "Espanha 2-1 França (semifinal)"),
        new(Euro, 2024, 1232551, "Espanha 2-1 Inglaterra (final)"),

        // Copa América 2024
        new(Copa, 2024, 1234030, "Argentina 1-0 Colômbia (final, prorrogação)"),
    ];
}
