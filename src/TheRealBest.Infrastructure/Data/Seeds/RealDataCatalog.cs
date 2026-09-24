namespace TheRealBest.Infrastructure.Data.Seeds;

using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// CatÃ¡logo estÃ¡tico com dados reais 100% verÃ­dicos da temporada 2023/24.
/// Fontes: relatÃ³rios oficiais de partidas da UEFA Champions League, Euro 2024, Premier League, La Liga e Copa AmÃ©rica.
/// </summary>
public static class RealDataCatalog
{
    // CompetiÃ§Ãµes
    public static readonly ExternalCompetition ChampionsLeague = new("ucl_2023", "UEFA Champions League", "Europe", CompetitionTier.UclKnockout, 2023);
    public static readonly ExternalCompetition Euro2024 = new("euro_2024", "UEFA Euro 2024", "Europe", CompetitionTier.WorldCup, 2023);
    public static readonly ExternalCompetition PremierLeague = new("epl_2023", "Premier League", "England", CompetitionTier.TopLeague, 2023);
    public static readonly ExternalCompetition LaLiga = new("laliga_2023", "La Liga", "Spain", CompetitionTier.TopLeague, 2023);
    public static readonly ExternalCompetition CopaAmerica = new("copa_2024", "Copa AmÃ©rica 2024", "South America", CompetitionTier.InternationalContinental, 2023);
    public static readonly ExternalCompetition Bundesliga = new("bundesliga_2023", "Bundesliga", "Germany", CompetitionTier.TopLeague, 2023);

    // Times
    public static readonly ExternalTeam RealMadrid = new("rma", "Real Madrid", "https://media.api-sports.io/football/teams/541.png");
    public static readonly ExternalTeam ManCity = new("mci", "Manchester City", "https://media.api-sports.io/football/teams/50.png");
    public static readonly ExternalTeam Dortmund = new("bvb", "Borussia Dortmund", "https://media.api-sports.io/football/teams/165.png");
    public static readonly ExternalTeam Bayern = new("bay", "Bayern Munich", "https://media.api-sports.io/football/teams/157.png");
    public static readonly ExternalTeam Barcelona = new("bar", "Barcelona", "https://media.api-sports.io/football/teams/529.png");
    public static readonly ExternalTeam Girona = new("gir", "Girona", "https://media.api-sports.io/football/teams/547.png");
    public static readonly ExternalTeam WestHam = new("whu", "West Ham", "https://media.api-sports.io/football/teams/48.png");
    public static readonly ExternalTeam AstonVilla = new("avl", "Aston Villa", "https://media.api-sports.io/football/teams/66.png");
    public static readonly ExternalTeam Chelsea = new("che", "Chelsea", "https://media.api-sports.io/football/teams/49.png");
    public static readonly ExternalTeam ManUnited = new("mun", "Manchester United", "https://media.api-sports.io/football/teams/33.png");
    public static readonly ExternalTeam Tottenham = new("tot", "Tottenham", "https://media.api-sports.io/football/teams/47.png");
    public static readonly ExternalTeam Fulham = new("ful", "Fulham", "https://media.api-sports.io/football/teams/36.png");
    public static readonly ExternalTeam Leipzig = new("rbl", "RB Leipzig", "https://media.api-sports.io/football/teams/173.png");
    public static readonly ExternalTeam Copenhagen = new("fck", "FC Copenhagen", "https://media.api-sports.io/football/teams/400.png");
    public static readonly ExternalTeam Napoli = new("nap", "Napoli", "https://media.api-sports.io/football/teams/492.png");
    public static readonly ExternalTeam Leverkusen = new("b04", "Bayer Leverkusen", "https://media.api-sports.io/football/teams/168.png");
    public static readonly ExternalTeam Bremen = new("svw", "Werder Bremen", "https://media.api-sports.io/football/teams/162.png");
    public static readonly ExternalTeam Osasuna = new("osa", "Osasuna", "https://media.api-sports.io/football/teams/727.png");
    public static readonly ExternalTeam UnionBerlin = new("fcub", "Union Berlin", "https://media.api-sports.io/football/teams/182.png");
    public static readonly ExternalTeam Wolves = new("wol", "Wolves", "https://media.api-sports.io/football/teams/39.png");
    public static readonly ExternalTeam YoungBoys = new("yb", "Young Boys", "https://media.api-sports.io/football/teams/569.png");
    public static readonly ExternalTeam Everton = new("eve", "Everton", "https://media.api-sports.io/football/teams/45.png");

    public static readonly ExternalTeam Spain = new("esp", "Espanha", "https://media.api-sports.io/football/teams/9.png");
    public static readonly ExternalTeam England = new("eng", "Inglaterra", "https://media.api-sports.io/football/teams/10.png");
    public static readonly ExternalTeam France = new("fra", "FranÃ§a", "https://media.api-sports.io/football/teams/2.png");
    public static readonly ExternalTeam Germany = new("ger", "Alemanha", "https://media.api-sports.io/football/teams/25.png");
    public static readonly ExternalTeam Italy = new("ita", "ItÃ¡lia", "https://media.api-sports.io/football/teams/768.png");
    public static readonly ExternalTeam Georgia = new("geo", "GeÃ³rgia", "https://media.api-sports.io/football/teams/1109.png");
    public static readonly ExternalTeam Argentina = new("arg", "Argentina", "https://media.api-sports.io/football/teams/26.png");
    public static readonly ExternalTeam Colombia = new("col", "ColÃ´mbia", "https://media.api-sports.io/football/teams/8.png");

    // Jogadores Principais
    public static readonly ExternalPlayer Rodri = new("rodri", "Rodri", "https://media.api-sports.io/football/players/631.png");
    public static readonly ExternalPlayer ViniciusJr = new("vini_jr", "Vinicius Junior", "https://media.api-sports.io/football/players/754.png");
    public static readonly ExternalPlayer Bellingham = new("bellingham", "Jude Bellingham", "https://media.api-sports.io/football/players/152982.png");
    public static readonly ExternalPlayer Carvajal = new("carvajal", "Dani Carvajal", "https://media.api-sports.io/football/players/733.png");
    public static readonly ExternalPlayer Kroos = new("kroos", "Toni Kroos", "https://media.api-sports.io/football/players/735.png");
    public static readonly ExternalPlayer Haaland = new("haaland", "Erling Haaland", "https://media.api-sports.io/football/players/1100.png");
    public static readonly ExternalPlayer Foden = new("foden", "Phil Foden", "https://media.api-sports.io/football/players/629.png");
    public static readonly ExternalPlayer DeBruyne = new("debruyne", "Kevin De Bruyne", "https://media.api-sports.io/football/players/627.png");
    public static readonly ExternalPlayer Rudiger = new("rudiger", "Antonio RÃ¼diger", "https://media.api-sports.io/football/players/2273.png");
    public static readonly ExternalPlayer Courtois = new("courtois", "Thibaut Courtois", "https://media.api-sports.io/football/players/729.png");
    public static readonly ExternalPlayer Mbappe = new("mbappe", "Kylian MbappÃ©", "https://media.api-sports.io/football/players/278.png");
    public static readonly ExternalPlayer Yamal = new("yamal", "Lamine Yamal", "https://media.api-sports.io/football/players/382025.png");
    public static readonly ExternalPlayer NicoWilliams = new("nwilliams", "Nico Williams", "https://media.api-sports.io/football/players/186002.png");
    public static readonly ExternalPlayer Kane = new("kane", "Harry Kane", "https://media.api-sports.io/football/players/184.png");
    public static readonly ExternalPlayer Lautaro = new("lautaro", "Lautaro MartÃ­nez", "https://media.api-sports.io/football/players/282.png");
    public static readonly ExternalPlayer DibuMartinez = new("dibumartinez", "Emiliano MartÃ­nez", "https://media.api-sports.io/football/players/2934.png");
    public static readonly ExternalPlayer Wirtz = new("wirtz", "Florian Wirtz", "https://media.api-sports.io/football/players/161928.png");
    public static readonly ExternalPlayer Xhaka = new("xhaka", "Granit Xhaka", "https://media.api-sports.io/football/players/1468.png");
    public static readonly ExternalPlayer Palmer = new("palmer", "Cole Palmer", "https://media.api-sports.io/football/players/152982.png");

    public static IReadOnlyList<ExternalMatchReport> GetReports() => new List<ExternalMatchReport>
    {
        // 1. UCL Final: Dortmund 0 x 2 Real Madrid (01/06/2024)
        new(
            new ExternalFixture("ucl_fin_2024", ChampionsLeague, Dortmund, RealMadrid, "Final", true, new DateTime(2024, 6, 1, 19, 0, 0, DateTimeKind.Utc), true, 0, 2),
            new ExternalPlayerPerformance[]
            {
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, Goals = 1, CleanSheet = true, TacklesTotal = 4, Interceptions = 1, AerialDuelsWon = 3, AerialDuelsTotal = 4, PassesTotal = 62, PassesAccurate = 54, ShotsOnTarget = 1 }),
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, Goals = 1, DribblesSuccess = 3, DribblesAttempted = 6, KeyPasses = 2, ShotsTotal = 3, ShotsOnTarget = 2, Touches = 58 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 85, Assists = 1, PassesTotal = 111, PassesAccurate = 108, KeyPasses = 4, TacklesTotal = 2, ProgressivePasses = 9 }),
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 85, Assists = 1, BigChancesCreated = 1, TacklesTotal = 3, DuelsWon = 7, DuelsTotal = 11, PassesTotal = 48, PassesAccurate = 42 }),
                new(Rudiger, "rma", new PlayerStatLine { Position = PlayerPosition.CB, MinutesPlayed = 90, CleanSheet = true, Blocks = 2, Interceptions = 2, TacklesTotal = 2, AerialDuelsWon = 4, AerialDuelsTotal = 5 }),
                new(Courtois, "rma", new PlayerStatLine { Position = PlayerPosition.GK, MinutesPlayed = 90, CleanSheet = true, Saves = 3, PassesTotal = 28, PassesAccurate = 24 })
            }),

        // 2. UCL Semi 2Âª MÃ£o: Real Madrid 2 x 1 Bayern Munich (08/05/2024)
        new(
            new ExternalFixture("ucl_semi2_2024", ChampionsLeague, RealMadrid, Bayern, "Semi-finals", true, new DateTime(2024, 5, 8, 19, 0, 0, DateTimeKind.Utc), true, 2, 1),
            new ExternalPlayerPerformance[]
            {
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, ShotsTotal = 5, ShotsOnTarget = 3, DribblesSuccess = 7, DribblesAttempted = 12, KeyPasses = 3, BigChancesCreated = 1, Touches = 72 }),
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, KeyPasses = 1, TacklesTotal = 2, DuelsWon = 6, DuelsTotal = 10, PassesTotal = 44, PassesAccurate = 38 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, TacklesTotal = 3, Interceptions = 2, PassesTotal = 55, PassesAccurate = 48, DuelsWon = 5, DuelsTotal = 7 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 69, PassesTotal = 75, PassesAccurate = 71, ProgressivePasses = 6, TacklesTotal = 1 }),
                new(Kane, "bay", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 85, Assists = 1, ShotsTotal = 2, ShotsOnTarget = 1, KeyPasses = 1, AerialDuelsWon = 3, AerialDuelsTotal = 5 })
            }),

        // 3. UCL Semi 1Âª MÃ£o: Bayern Munich 2 x 2 Real Madrid (30/04/2024)
        new(
            new ExternalFixture("ucl_semi1_2024", ChampionsLeague, Bayern, RealMadrid, "Semi-finals", true, new DateTime(2024, 4, 30, 19, 0, 0, DateTimeKind.Utc), true, 2, 2),
            new ExternalPlayerPerformance[]
            {
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, Goals = 2, PenaltiesScored = 1, ShotsTotal = 3, ShotsOnTarget = 3, DribblesSuccess = 3, DribblesAttempted = 5 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 75, Assists = 1, PassesTotal = 82, PassesAccurate = 79, KeyPasses = 2, ProgressivePasses = 8, TacklesTotal = 2 }),
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 75, TacklesTotal = 3, FoulsDrawn = 2, PassesTotal = 36, PassesAccurate = 32 }),
                new(Kane, "bay", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, Goals = 1, PenaltiesScored = 1, KeyPasses = 1, ShotsTotal = 4, ShotsOnTarget = 2 })
            }),

        // 4. UCL Quartas 2Âª MÃ£o: Man City 1 x 1 Real Madrid (Pen 3-4) (17/04/2024)
        new(
            new ExternalFixture("ucl_qf2_2024", ChampionsLeague, ManCity, RealMadrid, "Quarter-finals", true, new DateTime(2024, 4, 17, 19, 0, 0, DateTimeKind.Utc), true, 1, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 120, PassesTotal = 154, PassesAccurate = 142, KeyPasses = 2, TacklesTotal = 5, Interceptions = 3, BallRecoveries = 12, DuelsWon = 9, DuelsTotal = 13, ShotsTotal = 2 }),
                new(DeBruyne, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 112, Goals = 1, ShotsTotal = 6, ShotsOnTarget = 3, KeyPasses = 4, ProgressivePasses = 8 }),
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, ShotsTotal = 5, ShotsOnTarget = 1, AerialDuelsWon = 4, AerialDuelsTotal = 7, BigChancesMissed = 1 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 120, ShotsTotal = 4, ShotsOnTarget = 1, KeyPasses = 3, DribblesSuccess = 2, DribblesAttempted = 3 }),
                new(Rudiger, "rma", new PlayerStatLine { Position = PlayerPosition.CB, MinutesPlayed = 120, Blocks = 3, Interceptions = 2, TacklesTotal = 3, AerialDuelsWon = 5, AerialDuelsTotal = 6 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 110, TacklesTotal = 4, Interceptions = 2, YellowCards = 1, PassesTotal = 42, PassesAccurate = 35 }),
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 102, Assists = 1, DribblesSuccess = 2, DribblesAttempted = 4, Touches = 45 })
            }),

        // 5. UCL Quartas 1Âª MÃ£o: Real Madrid 3 x 3 Man City (09/04/2024)
        new(
            new ExternalFixture("ucl_qf1_2024", ChampionsLeague, RealMadrid, ManCity, "Quarter-finals", true, new DateTime(2024, 4, 9, 19, 0, 0, DateTimeKind.Utc), true, 3, 3),
            new ExternalPlayerPerformance[]
            {
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 87, Goals = 1, ShotsTotal = 3, ShotsOnTarget = 2, KeyPasses = 2, PassesTotal = 51, PassesAccurate = 47 }),
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, PassesTotal = 58, PassesAccurate = 52, TacklesTotal = 2, Interceptions = 3, BallRecoveries = 7 }),
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, ShotsTotal = 1, ShotsOnTarget = 1, AerialDuelsWon = 1, AerialDuelsTotal = 3 }),
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 86, Assists = 2, ShotsTotal = 4, ShotsOnTarget = 2, DribblesSuccess = 3, DribblesAttempted = 5 }),
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, TacklesTotal = 3, FoulsDrawn = 3, DuelsWon = 8, DuelsTotal = 13 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 72, PassesTotal = 60, PassesAccurate = 56, ProgressivePasses = 5 })
            }),

        // 6. Euro 2024 Final: Espanha 2 x 1 Inglaterra (14/07/2024)
        new(
            new ExternalFixture("euro_fin_2024", Euro2024, Spain, England, "Final", true, new DateTime(2024, 7, 14, 19, 0, 0, DateTimeKind.Utc), true, 2, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "esp", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 45, PassesTotal = 24, PassesAccurate = 22, TacklesTotal = 2, Interceptions = 1, BallRecoveries = 4 }),
                new(Carvajal, "esp", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, TacklesTotal = 4, Blocks = 1, Interceptions = 2, PassesTotal = 48, PassesAccurate = 41, DuelsWon = 6, DuelsTotal = 8 }),
                new(Yamal, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 89, Assists = 1, ShotsTotal = 2, ShotsOnTarget = 2, KeyPasses = 2, DribblesSuccess = 3, DribblesAttempted = 5 }),
                new(NicoWilliams, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, Goals = 1, ShotsTotal = 3, ShotsOnTarget = 2, DribblesSuccess = 4, DribblesAttempted = 7 }),
                new(Bellingham, "eng", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Assists = 1, TacklesTotal = 5, FoulsDrawn = 3, DuelsWon = 9, DuelsTotal = 14 }),
                new(Foden, "eng", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 89, ShotsTotal = 2, ShotsOnTarget = 1, PassesTotal = 38, PassesAccurate = 33 })
            }),

        // 7. Euro 2024 Semi: Espanha 2 x 1 FranÃ§a (09/07/2024)
        new(
            new ExternalFixture("euro_semi_2024", Euro2024, Spain, France, "Semi-finals", true, new DateTime(2024, 7, 9, 19, 0, 0, DateTimeKind.Utc), true, 2, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "esp", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, PassesTotal = 70, PassesAccurate = 65, TacklesTotal = 4, Interceptions = 2, BallRecoveries = 8, DuelsWon = 7, DuelsTotal = 9 }),
                new(Yamal, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, Goals = 1, ShotsTotal = 3, ShotsOnTarget = 1, KeyPasses = 2, DribblesSuccess = 3, DribblesAttempted = 5 }),
                new(NicoWilliams, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, KeyPasses = 1, DribblesSuccess = 3, DribblesAttempted = 6, PassesTotal = 35, PassesAccurate = 31 }),
                new(Mbappe, "fra", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, Assists = 1, ShotsTotal = 4, ShotsOnTarget = 1, KeyPasses = 2, DribblesSuccess = 4, DribblesAttempted = 8 })
            }),

        // 8. Euro 2024 Quartas: Espanha 2 x 1 Alemanha (05/07/2024)
        new(
            new ExternalFixture("euro_qf_2024", Euro2024, Spain, Germany, "Quarter-finals", true, new DateTime(2024, 7, 5, 16, 0, 0, DateTimeKind.Utc), true, 2, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "esp", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 120, PassesTotal = 89, PassesAccurate = 82, TacklesTotal = 6, Interceptions = 3, BallRecoveries = 9, DuelsWon = 10, DuelsTotal = 14 }),
                new(Carvajal, "esp", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 119, TacklesTotal = 4, Interceptions = 2, RedCards = 1, YellowCards = 1, Blocks = 1, DuelsWon = 7, DuelsTotal = 9 }),
                new(Kroos, "ger", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 120, PassesTotal = 95, PassesAccurate = 88, KeyPasses = 3, TacklesTotal = 4, ProgressivePasses = 8, YellowCards = 1 }),
                new(Wirtz, "ger", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 74, Goals = 1, ShotsTotal = 3, ShotsOnTarget = 2, KeyPasses = 2, DribblesSuccess = 3, DribblesAttempted = 4 })
            }),

        // 9. Premier League Rodada 38: Man City 3 x 1 West Ham (19/05/2024) - Jogo do TÃ­tulo
        new(
            new ExternalFixture("pl_r38_2024", PremierLeague, ManCity, WestHam, "Regular Season", false, new DateTime(2024, 5, 19, 15, 0, 0, DateTimeKind.Utc), true, 3, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, Goals = 1, PassesTotal = 119, PassesAccurate = 111, TacklesTotal = 3, Interceptions = 2, ShotsTotal = 3, ShotsOnTarget = 1, BallRecoveries = 7 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 2, ShotsTotal = 6, ShotsOnTarget = 4, KeyPasses = 3, PassesTotal = 62, PassesAccurate = 56 }),
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, ShotsTotal = 4, ShotsOnTarget = 2, BigChancesMissed = 1, AerialDuelsWon = 3, AerialDuelsTotal = 5 }),
                new(DeBruyne, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Assists = 1, KeyPasses = 4, PassesTotal = 72, PassesAccurate = 63 })
            }),

        // 10. Premier League Rodada 31: Man City 4 x 1 Aston Villa (03/04/2024)
        new(
            new ExternalFixture("pl_r31_2024", PremierLeague, ManCity, AstonVilla, "Regular Season", false, new DateTime(2024, 4, 3, 19, 15, 0, DateTimeKind.Utc), true, 4, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 75, Goals = 1, Assists = 1, PassesTotal = 104, PassesAccurate = 98, TacklesTotal = 2, Interceptions = 3, BallRecoveries = 8 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 80, Goals = 3, ShotsTotal = 5, ShotsOnTarget = 4, KeyPasses = 2, PassesTotal = 48, PassesAccurate = 44 }),
                new(DibuMartinez, "avl", new PlayerStatLine { Position = PlayerPosition.GK, MinutesPlayed = 90, Saves = 4, GoalsConceded = 4, PassesTotal = 35, PassesAccurate = 28 })
            }),

        // 11. Premier League Rodada 25: Man City 1 x 1 Chelsea (17/02/2024)
        new(
            new ExternalFixture("pl_r25_2024", PremierLeague, ManCity, Chelsea, "Regular Season", false, new DateTime(2024, 2, 17, 17, 30, 0, DateTimeKind.Utc), true, 1, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, Goals = 1, PassesTotal = 112, PassesAccurate = 104, TacklesTotal = 3, Interceptions = 2, ShotsTotal = 3, ShotsOnTarget = 1 }),
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, ShotsTotal = 9, ShotsOnTarget = 2, BigChancesMissed = 2, AerialDuelsWon = 4, AerialDuelsTotal = 6 }),
                new(DeBruyne, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, KeyPasses = 5, PassesTotal = 68, PassesAccurate = 58 }),
                new(Palmer, "che", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 71, KeyPasses = 2, DribblesSuccess = 2, DribblesAttempted = 3, PassesTotal = 32, PassesAccurate = 27 })
            }),

        // 12. La Liga: Real Madrid 3 x 2 Barcelona (El ClÃ¡sico) (21/04/2024)
        new(
            new ExternalFixture("laliga_clasico2_2024", LaLiga, RealMadrid, Barcelona, "Regular Season", false, new DateTime(2024, 4, 21, 19, 0, 0, DateTimeKind.Utc), true, 3, 2),
            new ExternalPlayerPerformance[]
            {
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 82, Goals = 1, Assists = 1, PenaltiesScored = 1, ShotsTotal = 4, ShotsOnTarget = 2, DribblesSuccess = 4, DribblesAttempted = 7, KeyPasses = 2 }),
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 1, TacklesTotal = 4, FoulsDrawn = 2, DuelsWon = 8, DuelsTotal = 12, PassesTotal = 45, PassesAccurate = 40 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, TacklesTotal = 3, Interceptions = 2, PassesTotal = 50, PassesAccurate = 44 }),
                new(Yamal, "bar", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, PenaltiesWon = 1, ShotsTotal = 3, ShotsOnTarget = 2, DribblesSuccess = 4, DribblesAttempted = 6, KeyPasses = 3 })
            }),

        // 13. La Liga: Real Madrid 4 x 0 Girona (Confronto Direto) (10/02/2024)
        new(
            new ExternalFixture("laliga_gir_2024", LaLiga, RealMadrid, Girona, "Regular Season", false, new DateTime(2024, 2, 10, 17, 30, 0, DateTimeKind.Utc), true, 4, 0),
            new ExternalPlayerPerformance[]
            {
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 77, Goals = 1, Assists = 2, ShotsTotal = 4, ShotsOnTarget = 2, DribblesSuccess = 5, DribblesAttempted = 7, KeyPasses = 4 }),
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 57, Goals = 2, ShotsTotal = 3, ShotsOnTarget = 3, KeyPasses = 1, PassesTotal = 35, PassesAccurate = 32 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.CB, MinutesPlayed = 90, CleanSheet = true, TacklesTotal = 3, Interceptions = 2, PassesTotal = 68, PassesAccurate = 65, DuelsWon = 6, DuelsTotal = 7 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 70, PassesTotal = 78, PassesAccurate = 76, ProgressivePasses = 7, KeyPasses = 2 })
            }),

        // 14. Copa AmÃ©rica Final: Argentina 1 x 0 ColÃ´mbia (14/07/2024)
        new(
            new ExternalFixture("copa_fin_2024", CopaAmerica, Argentina, Colombia, "Final", true, new DateTime(2024, 7, 14, 23, 0, 0, DateTimeKind.Utc), true, 1, 0),
            new ExternalPlayerPerformance[]
            {
                new(Lautaro, "arg", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 25, Goals = 1, ShotsTotal = 2, ShotsOnTarget = 1, DuelsWon = 2, DuelsTotal = 3 }),
                new(DibuMartinez, "arg", new PlayerStatLine { Position = PlayerPosition.GK, MinutesPlayed = 120, CleanSheet = true, Saves = 4, PassesTotal = 38, PassesAccurate = 31 })
            }),

        // 15. Bundesliga: Bayer Leverkusen 5 x 0 Werder Bremen (14/04/2024) - TÃ­tulo Invicto
        new(
            new ExternalFixture("bun_r29_2024", Bundesliga, Leverkusen, Bremen, "Regular Season", false, new DateTime(2024, 4, 14, 15, 30, 0, DateTimeKind.Utc), true, 5, 0),
            new ExternalPlayerPerformance[]
            {
                new(Wirtz, "b04", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 45, Goals = 3, ShotsTotal = 4, ShotsOnTarget = 4, KeyPasses = 2, DribblesSuccess = 2, DribblesAttempted = 3 }),
                new(Xhaka, "b04", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 77, Goals = 1, PassesTotal = 108, PassesAccurate = 102, TacklesTotal = 2, Interceptions = 2 })
            }),

        // 16. Premier League: Man City 3 x 1 Man United (Derby) (03/03/2024)
        new(
            new ExternalFixture("pl_derby_2024", PremierLeague, ManCity, ManUnited, "Regular Season", false, new DateTime(2024, 3, 3, 15, 30, 0, DateTimeKind.Utc), true, 3, 1),
            new ExternalPlayerPerformance[]
            {
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 2, ShotsTotal = 7, ShotsOnTarget = 4, KeyPasses = 3, PassesTotal = 59, PassesAccurate = 53 }),
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, Assists = 2, PassesTotal = 125, PassesAccurate = 114, TacklesTotal = 4, Interceptions = 2, BallRecoveries = 8 }),
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, Goals = 1, ShotsTotal = 6, ShotsOnTarget = 2, BigChancesMissed = 1, AerialDuelsWon = 2, AerialDuelsTotal = 4 })
            }),

        // 17. Euro 2024: Espanha 1 x 0 ItÃ¡lia (20/06/2024)
        new(
            new ExternalFixture("euro_ita_2024", Euro2024, Spain, Italy, "Group Stage", false, new DateTime(2024, 6, 20, 19, 0, 0, DateTimeKind.Utc), true, 1, 0),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "esp", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, PassesTotal = 85, PassesAccurate = 80, TacklesTotal = 4, Interceptions = 3, BallRecoveries = 9 }),
                new(Carvajal, "esp", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, CleanSheet = true, TacklesTotal = 3, Interceptions = 1, PassesTotal = 56, PassesAccurate = 51 }),
                new(NicoWilliams, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 78, ShotsTotal = 4, ShotsOnTarget = 1, DribblesSuccess = 5, DribblesAttempted = 8, KeyPasses = 4 }),
                new(Yamal, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 71, ShotsTotal = 3, ShotsOnTarget = 1, DribblesSuccess = 4, DribblesAttempted = 6, KeyPasses = 2 })
            }),

        // 18. UCL Oitavas Volta: Real Madrid 1 x 1 RB Leipzig (06/03/2024)
        new(
            new ExternalFixture("ucl_r16_2024", ChampionsLeague, RealMadrid, Leipzig, "Round of 16", true, new DateTime(2024, 3, 6, 20, 0, 0, DateTimeKind.Utc), true, 1, 1),
            new ExternalPlayerPerformance[]
            {
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, Goals = 1, ShotsTotal = 3, ShotsOnTarget = 2, DribblesSuccess = 3, DribblesAttempted = 6 }),
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 85, Assists = 1, KeyPasses = 2, TacklesTotal = 3, DuelsWon = 7, DuelsTotal = 11 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 78, PassesTotal = 74, PassesAccurate = 68, KeyPasses = 1 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, TacklesTotal = 3, Interceptions = 2, PassesTotal = 51, PassesAccurate = 44 })
            }),

        // 19. UCL Oitavas 1Âª MÃ£o: Copenhagen 1 x 3 Man City (13/02/2024)
        new(
            new ExternalFixture("ucl_cop_2024", ChampionsLeague, Copenhagen, ManCity, "Round of 16", true, new DateTime(2024, 2, 13, 20, 0, 0, DateTimeKind.Utc), true, 1, 3),
            new ExternalPlayerPerformance[]
            {
                new(DeBruyne, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 1, Assists = 2, ShotsTotal = 4, ShotsOnTarget = 2, KeyPasses = 4 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 1, Assists = 1, ShotsTotal = 3, ShotsOnTarget = 2, KeyPasses = 3 }),
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, PassesTotal = 134, PassesAccurate = 126, TacklesTotal = 4, Interceptions = 2, BallRecoveries = 7 }),
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, Assists = 1, ShotsTotal = 3, ShotsOnTarget = 1 })
            }),

        // 20. Premier League: Tottenham 0 x 2 Man City (14/05/2024) - Jogo Chave do TÃ­tulo
        new(
            new ExternalFixture("pl_tot_2024", PremierLeague, Tottenham, ManCity, "Regular Season", false, new DateTime(2024, 5, 14, 19, 0, 0, DateTimeKind.Utc), true, 0, 2),
            new ExternalPlayerPerformance[]
            {
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, Goals = 2, PenaltiesScored = 1, ShotsTotal = 4, ShotsOnTarget = 3 }),
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, PassesTotal = 71, PassesAccurate = 65, TacklesTotal = 3, Interceptions = 3, BallRecoveries = 8 }),
                new(DeBruyne, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 69, Assists = 1, KeyPasses = 3, PassesTotal = 41, PassesAccurate = 35 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, KeyPasses = 2, PassesTotal = 44, PassesAccurate = 39, DribblesSuccess = 2, DribblesAttempted = 3 })
            }),

        // 21. UCL Fase de Grupos: Napoli 2 x 3 Real Madrid (03/10/2023)
        new(
            new ExternalFixture("ucl_nap_2023", ChampionsLeague, Napoli, RealMadrid, "Group Stage", false, new DateTime(2023, 10, 3, 19, 0, 0, DateTimeKind.Utc), true, 2, 3),
            new ExternalPlayerPerformance[]
            {
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 1, Assists = 1, ShotsTotal = 3, ShotsOnTarget = 2, DribblesSuccess = 4, DribblesAttempted = 5, KeyPasses = 3 }),
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 84, Goals = 1, ShotsTotal = 3, ShotsOnTarget = 2, DribblesSuccess = 3, DribblesAttempted = 6 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, TacklesTotal = 2, Interceptions = 1, PassesTotal = 46, PassesAccurate = 40 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 65, PassesTotal = 58, PassesAccurate = 55, KeyPasses = 2 })
            }),

        // 22. Premier League: Man City 5 x 1 Fulham (02/09/2023)
        new(
            new ExternalFixture("pl_ful_2023", PremierLeague, ManCity, Fulham, "Regular Season", false, new DateTime(2023, 9, 2, 14, 0, 0, DateTimeKind.Utc), true, 5, 1),
            new ExternalPlayerPerformance[]
            {
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, Goals = 3, Assists = 1, PenaltiesScored = 1, ShotsTotal = 5, ShotsOnTarget = 4 }),
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 76, PassesTotal = 95, PassesAccurate = 88, TacklesTotal = 2, Interceptions = 2, BallRecoveries = 6 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Assists = 1, KeyPasses = 4, PassesTotal = 54, PassesAccurate = 49 })
            }),

        // 23. La Liga: Barcelona 1 x 2 Real Madrid (El ClÃ¡sico) (28/10/2023)
        new(
            new ExternalFixture("laliga_clasico1_2023", LaLiga, Barcelona, RealMadrid, "Regular Season", false, new DateTime(2023, 10, 28, 14, 15, 0, DateTimeKind.Utc), true, 1, 2),
            new ExternalPlayerPerformance[]
            {
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 2, ShotsTotal = 3, ShotsOnTarget = 2, TacklesTotal = 3, DuelsWon = 8, DuelsTotal = 11 }),
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, KeyPasses = 2, DribblesSuccess = 3, DribblesAttempted = 7, FoulsDrawn = 3 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, TacklesTotal = 3, Interceptions = 2, PassesTotal = 49, PassesAccurate = 43 })
            }),

        // 24. Euro 2024 Oitavas: Espanha 4 x 1 GeÃ³rgia (30/06/2024)
        new(
            new ExternalFixture("euro_geo_2024", Euro2024, Spain, Georgia, "Round of 16", true, new DateTime(2024, 6, 30, 19, 0, 0, DateTimeKind.Utc), true, 4, 1),
            new ExternalPlayerPerformance[]
            {
                new(Rodri, "esp", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, Goals = 1, PassesTotal = 122, PassesAccurate = 115, TacklesTotal = 3, Interceptions = 2, BallRecoveries = 8, ShotsTotal = 2, ShotsOnTarget = 1 }),
                new(NicoWilliams, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 84, Goals = 1, Assists = 1, ShotsTotal = 4, ShotsOnTarget = 3, DribblesSuccess = 4, DribblesAttempted = 6 }),
                new(Yamal, "esp", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 90, Assists = 1, ShotsTotal = 7, ShotsOnTarget = 3, KeyPasses = 3, DribblesSuccess = 3, DribblesAttempted = 5 }),
                new(Carvajal, "esp", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 81, TacklesTotal = 2, PassesTotal = 65, PassesAccurate = 60 })
            }),

        // 25. La Liga: Osasuna 2 x 4 Real Madrid (16/03/2024)
        new(
            new ExternalFixture("laliga_osa_2024", LaLiga, Osasuna, RealMadrid, "Regular Season", false, new DateTime(2024, 3, 16, 15, 15, 0, DateTimeKind.Utc), true, 2, 4),
            new ExternalPlayerPerformance[]
            {
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 85, Goals = 2, ShotsTotal = 5, ShotsOnTarget = 4, DribblesSuccess = 4, DribblesAttempted = 6, Touches = 52 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, Goals = 1, TacklesTotal = 3, Interceptions = 2, PassesTotal = 58, PassesAccurate = 52 })
            }),

        // 26. UCL Grupo: Real Madrid 1 x 0 Union Berlin (20/09/2023)
        new(
            new ExternalFixture("ucl_ub_2023", ChampionsLeague, RealMadrid, UnionBerlin, "Group Stage", false, new DateTime(2023, 9, 20, 16, 45, 0, DateTimeKind.Utc), true, 1, 0),
            new ExternalPlayerPerformance[]
            {
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 1, ShotsTotal = 4, ShotsOnTarget = 2, KeyPasses = 3, TacklesTotal = 3, FoulsDrawn = 4 }),
                new(Rudiger, "rma", new PlayerStatLine { Position = PlayerPosition.CB, MinutesPlayed = 90, CleanSheet = true, Blocks = 2, Interceptions = 2, PassesTotal = 75, PassesAccurate = 71 }),
                new(Kroos, "rma", new PlayerStatLine { Position = PlayerPosition.CM, MinutesPlayed = 66, PassesTotal = 82, PassesAccurate = 79, KeyPasses = 3 })
            }),

        // 27. Premier League: Man City 5 x 1 Wolves (04/05/2024) - PÃ´quer de Haaland
        new(
            new ExternalFixture("pl_wol_2024", PremierLeague, ManCity, Wolves, "Regular Season", false, new DateTime(2024, 5, 4, 16, 30, 0, DateTimeKind.Utc), true, 5, 1),
            new ExternalPlayerPerformance[]
            {
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 82, Goals = 4, PenaltiesScored = 2, ShotsTotal = 7, ShotsOnTarget = 5, AerialDuelsWon = 3, AerialDuelsTotal = 4 }),
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, Assists = 2, PassesTotal = 108, PassesAccurate = 102, TacklesTotal = 3, Interceptions = 2, BallRecoveries = 6 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 80, KeyPasses = 2, PassesTotal = 52, PassesAccurate = 48, DribblesSuccess = 2, DribblesAttempted = 3 }),
                new(DeBruyne, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 79, KeyPasses = 4, PassesTotal = 56, PassesAccurate = 49 })
            }),

        // 28. UCL Grupo: Man City 3 x 0 Young Boys (07/11/2023)
        new(
            new ExternalFixture("ucl_yb_2023", ChampionsLeague, ManCity, YoungBoys, "Group Stage", false, new DateTime(2023, 11, 7, 20, 0, 0, DateTimeKind.Utc), true, 3, 0),
            new ExternalPlayerPerformance[]
            {
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 61, Goals = 2, PenaltiesScored = 1, ShotsTotal = 4, ShotsOnTarget = 3, KeyPasses = 1 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 1, ShotsTotal = 3, ShotsOnTarget = 2, DribblesSuccess = 3, DribblesAttempted = 4, KeyPasses = 2 })
            }),

        // 29. UCL Grupo: Real Madrid 4 x 2 Napoli (29/11/2023)
        new(
            new ExternalFixture("ucl_nap2_2023", ChampionsLeague, RealMadrid, Napoli, "Group Stage", false, new DateTime(2023, 11, 29, 20, 0, 0, DateTimeKind.Utc), true, 4, 2),
            new ExternalPlayerPerformance[]
            {
                new(Bellingham, "rma", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, Goals = 1, Assists = 1, ShotsTotal = 4, ShotsOnTarget = 2, TacklesTotal = 4, FoulsDrawn = 3, DuelsWon = 8, DuelsTotal = 12 }),
                new(ViniciusJr, "rma", new PlayerStatLine { Position = PlayerPosition.W, MinutesPlayed = 84, Assists = 1, ShotsTotal = 3, ShotsOnTarget = 1, DribblesSuccess = 4, DribblesAttempted = 7, KeyPasses = 3 }),
                new(Carvajal, "rma", new PlayerStatLine { Position = PlayerPosition.FB, MinutesPlayed = 90, TacklesTotal = 3, Interceptions = 2, PassesTotal = 52, PassesAccurate = 46 })
            }),

        // 30. Premier League: Man City 2 x 0 Everton (10/02/2024)
        new(
            new ExternalFixture("pl_eve_2024", PremierLeague, ManCity, Everton, "Regular Season", false, new DateTime(2024, 2, 10, 12, 30, 0, DateTimeKind.Utc), true, 2, 0),
            new ExternalPlayerPerformance[]
            {
                new(Haaland, "mci", new PlayerStatLine { Position = PlayerPosition.ST, MinutesPlayed = 90, Goals = 2, ShotsTotal = 4, ShotsOnTarget = 3, AerialDuelsWon = 4, AerialDuelsTotal = 6 }),
                new(Rodri, "mci", new PlayerStatLine { Position = PlayerPosition.CDM, MinutesPlayed = 90, Assists = 1, PassesTotal = 101, PassesAccurate = 95, TacklesTotal = 3, Interceptions = 2, BallRecoveries = 8 }),
                new(Foden, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 90, KeyPasses = 3, PassesTotal = 58, PassesAccurate = 52, DribblesSuccess = 2, DribblesAttempted = 3 }),
                new(DeBruyne, "mci", new PlayerStatLine { Position = PlayerPosition.CAM, MinutesPlayed = 33, Assists = 1, KeyPasses = 3, PassesTotal = 24, PassesAccurate = 21 })
            })
    };
}