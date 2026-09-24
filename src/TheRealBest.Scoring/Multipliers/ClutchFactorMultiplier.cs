namespace TheRealBest.Scoring.Multipliers;

/// <summary>
/// Fator decisivo (W_clutch), seção 4.3 da especificação.
/// A especificação prevê o cálculo lance a lance (placar e minuto de cada ação). Enquanto não há
/// eventos por minuto na ingestão, o fator é derivado do placar final:
/// jogo decidido por 1 gol ou empatado = equilibrado até o fim; diferença de 3+ gols = junk time.
/// </summary>
public static class ClutchFactorMultiplier
{
    public const decimal Tight = 1.25m;
    public const decimal Open = 1.00m;
    public const decimal JunkTime = 0.70m;

    public const int TightMaxMargin = 1;
    public const int JunkTimeMinMargin = 3;

    public static decimal ForFinalScore(int teamScore, int opponentScore)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(teamScore);
        ArgumentOutOfRangeException.ThrowIfNegative(opponentScore);

        var margin = Math.Abs(teamScore - opponentScore);
        return margin switch
        {
            <= TightMaxMargin => Tight,
            >= JunkTimeMinMargin => JunkTime,
            _ => Open,
        };
    }
}
