namespace TheRealBest.Scoring.Calculators;

/// <summary>
/// FatorMinutos, seção 5 da especificação:
/// M/90 se M &lt; 60; 1.0 se 60 ≤ M ≤ 90; 1.0 + (M − 90)/180 na prorrogação.
/// </summary>
public static class MinutesFactorCalculator
{
    public const int FullFactorFromMinute = 60;
    public const int RegulationMinutes = 90;

    public static decimal Calculate(int minutesPlayed)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minutesPlayed);

        var factor = minutesPlayed switch
        {
            < FullFactorFromMinute => minutesPlayed / (decimal)RegulationMinutes,
            <= RegulationMinutes => 1.0m,
            _ => 1.0m + (minutesPlayed - RegulationMinutes) / 180m,
        };

        return ScoringMath.Round4(factor);
    }
}
