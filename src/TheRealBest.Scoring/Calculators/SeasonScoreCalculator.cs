namespace TheRealBest.Scoring.Calculators;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// Calcula o Fair Season Score (FSS):
/// FSS = (Σ MPS_i × W_torneio_i / Σ W_torneio_i) × FatorPresença, com FatorPresença = min(1, minutos / 2200)^0.5.
/// </summary>
/// <remarks>
/// A especificação descreve uma soma (Σ MPS × W), que premia volume de jogos acima de qualidade.
/// A média ponderada mantém a escala de 0 a 100 e deixa o FatorPresença como única proteção contra amostras pequenas.
/// </remarks>
public static class SeasonScoreCalculator
{
    public const int FullPresenceMinutes = 2200;

    public static SeasonScore Calculate(IEnumerable<MatchPerformanceScore> scores, int totalSeasonMinutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalSeasonMinutes);

        var counted = scores.Where(s => s.CountsTowardsSeason).ToList();
        if (counted.Count == 0)
        {
            return SeasonScore.Empty;
        }

        var weightSum = counted.Sum(s => s.TournamentMultiplier);
        var weightedSum = counted.Sum(s => s.FinalMps * s.TournamentMultiplier);
        var weightedAverage = weightSum == 0m ? 0m : weightedSum / weightSum;
        var presenceFactor = CalculatePresenceFactor(totalSeasonMinutes);

        return new SeasonScore(
            counted.Count,
            MpsAverage: ScoringMath.Round4(counted.Average(s => s.FinalMps)),
            WeightedMpsSum: ScoringMath.Round4(weightedSum),
            WeightedMpsAverage: ScoringMath.Round4(weightedAverage),
            PresenceFactor: presenceFactor,
            Fss: ScoringMath.Round4(weightedAverage * presenceFactor));
    }

    public static decimal CalculatePresenceFactor(int totalSeasonMinutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalSeasonMinutes);

        var ratio = Math.Min(1.0, totalSeasonMinutes / (double)FullPresenceMinutes);
        return ScoringMath.Round4((decimal)Math.Sqrt(ratio));
    }
}
