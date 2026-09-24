namespace TheRealBest.Domain.ValueObjects;

/// <summary>
/// Resultado do cálculo do Fair Season Score (FSS) de um jogador.
/// FSS = WeightedMpsAverage × PresenceFactor.
/// </summary>
/// <param name="MatchesCounted">Partidas que entraram no cálculo (as que têm CountsTowardsSeason).</param>
/// <param name="MpsAverage">Média simples do MPS das partidas contadas.</param>
/// <param name="WeightedMpsSum">Σ MPS × W_torneio.</param>
/// <param name="WeightedMpsAverage">Σ MPS × W_torneio / Σ W_torneio.</param>
/// <param name="PresenceFactor">min(1, minutos / 2200)^0.5.</param>
/// <param name="Fss">Fair Season Score final, na escala de 0 a 100.</param>
/// <param name="IsRankingEligible">
/// Verdadeiro quando o jogador atinge o mínimo de partidas contadas e de minutos para aparecer no ranking.
/// </param>
public sealed record SeasonScore(
    int MatchesCounted,
    decimal MpsAverage,
    decimal WeightedMpsSum,
    decimal WeightedMpsAverage,
    decimal PresenceFactor,
    decimal Fss,
    bool IsRankingEligible
)
{
    public static SeasonScore Empty => new(0, 0m, 0m, 0m, 0m, 0m, IsRankingEligible: false);
}
