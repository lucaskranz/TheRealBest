namespace TheRealBest.Scoring.Multipliers;

/// <summary>
/// Peso da força do adversário (W_adversário), seção 4.2 da especificação.
/// A especificação usa a posição no ranking mundial; como armazenamos o rating Elo (escala ClubElo),
/// as faixas foram convertidas em limiares de rating equivalentes.
/// </summary>
public static class OpponentStrengthMultiplier
{
    /// <summary>Rating aproximado do 10º clube no ClubElo.</summary>
    public const int Top10EloThreshold = 1880;

    /// <summary>Rating aproximado do 30º clube no ClubElo.</summary>
    public const int Top30EloThreshold = 1780;

    /// <summary>Abaixo disso: nível de zona de rebaixamento das grandes ligas ou divisões inferiores.</summary>
    public const int MidTableEloThreshold = 1600;

    public const decimal Top10 = 1.20m;
    public const decimal Top30 = 1.10m;
    public const decimal MidTable = 1.00m;
    public const decimal Weak = 0.90m;

    /// <param name="opponentElo">Rating na data do jogo. Nulo (seleções ou fonte indisponível) = neutro, não "fraco".</param>
    public static decimal For(int? opponentElo) => opponentElo switch
    {
        null => MidTable,
        >= Top10EloThreshold => Top10,
        >= Top30EloThreshold => Top30,
        >= MidTableEloThreshold => MidTable,
        _ => Weak,
    };
}
