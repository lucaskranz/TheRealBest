namespace TheRealBest.Scoring.Calculators;

/// <summary>
/// Arredondamentos padronizados: o algoritmo precisa ser determinístico e reprodutível.
/// </summary>
internal static class ScoringMath
{
    public static decimal Round4(decimal value) => Math.Round(value, 4, MidpointRounding.AwayFromZero);

    public static decimal Round2(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
