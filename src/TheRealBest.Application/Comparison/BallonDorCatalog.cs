namespace TheRealBest.Application.Comparison;

/// <summary>Um indicado ao prêmio, na colocação oficial.</summary>
/// <param name="ExternalApiId">Id do jogador na API-Football; nulo quando o jogador não aparece em nenhuma partida importada.</param>
public sealed record BallonDorNominee(int OfficialRank, string Name, string Club, string? ExternalApiId);

/// <summary>Classificação oficial de uma edição da Bola de Ouro e a temporada que ela avalia.</summary>
public sealed record BallonDorEdition(
    int Year,
    int SeasonYear,
    string SourceName,
    string SourceUrl,
    IReadOnlyList<BallonDorNominee> Nominees);

/// <summary>
/// Classificações oficiais da Bola de Ouro (France Football). Dado estático e citado: nada aqui é estimado.
/// </summary>
public static class BallonDorCatalog
{
    /// <summary>
    /// Edição 2024 (ciclo 2023/24). Dovbyk e Hummels empataram em 29º, com zero pontos.
    /// Lookman, Ødegaard, Çalhanoğlu e Vitinha não jogaram nenhuma das partidas importadas.
    /// </summary>
    public static BallonDorEdition Edition2024 { get; } = new(
        Year: 2024,
        SeasonYear: 2023,
        SourceName: "Wikipedia — 2024 Ballon d'Or",
        SourceUrl: "https://en.wikipedia.org/wiki/2024_Ballon_d%27Or",
        Nominees:
        [
            new(1, "Rodri", "Manchester City", "44"),
            new(2, "Vinícius Júnior", "Real Madrid", "762"),
            new(3, "Jude Bellingham", "Real Madrid", "129718"),
            new(4, "Dani Carvajal", "Real Madrid", "733"),
            new(5, "Erling Haaland", "Manchester City", "1100"),
            new(6, "Kylian Mbappé", "Paris Saint-Germain", "278"),
            new(7, "Lautaro Martínez", "Inter", "217"),
            new(8, "Lamine Yamal", "Barcelona", "386828"),
            new(9, "Toni Kroos", "Real Madrid", "752"),
            new(10, "Harry Kane", "Bayern München", "184"),
            new(11, "Phil Foden", "Manchester City", "631"),
            new(12, "Florian Wirtz", "Bayer Leverkusen", "203224"),
            new(13, "Dani Olmo", "RB Leipzig", "1323"),
            new(14, "Ademola Lookman", "Atalanta", null),
            new(15, "Nico Williams", "Athletic Club", "183799"),
            new(16, "Granit Xhaka", "Bayer Leverkusen", "1464"),
            new(17, "Federico Valverde", "Real Madrid", "756"),
            new(18, "Emiliano Martínez", "Aston Villa", "19599"),
            new(19, "Martin Ødegaard", "Arsenal", null),
            new(20, "Hakan Çalhanoğlu", "Inter", null),
            new(21, "Bukayo Saka", "Arsenal", "1460"),
            new(22, "Antonio Rüdiger", "Real Madrid", "2285"),
            new(23, "Rúben Dias", "Manchester City", "567"),
            new(24, "William Saliba", "Arsenal", "22090"),
            new(25, "Cole Palmer", "Chelsea", "152982"),
            new(26, "Declan Rice", "Arsenal", "2937"),
            new(27, "Vitinha", "Paris Saint-Germain", null),
            new(28, "Alejandro Grimaldo", "Bayer Leverkusen", "563"),
            new(29, "Artem Dovbyk", "Girona", "15811"),
            new(29, "Mats Hummels", "Borussia Dortmund", "501"),
        ]);

    public static IReadOnlyList<BallonDorEdition> All { get; } = [Edition2024];

    public static BallonDorEdition? ForYear(int year) => All.FirstOrDefault(e => e.Year == year);
}
