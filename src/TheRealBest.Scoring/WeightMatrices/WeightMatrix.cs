namespace TheRealBest.Scoring.WeightMatrices;

using TheRealBest.Domain.Enums;

/// <summary>
/// Pesos de cada ação para uma função tática. Ações ausentes valem zero ("—" na especificação).
/// Penalidades são armazenadas com sinal negativo.
/// </summary>
public sealed class WeightMatrix(IReadOnlyDictionary<ActionType, decimal> weights)
{
    public IReadOnlyDictionary<ActionType, decimal> Weights { get; } = weights;

    public decimal WeightFor(ActionType action) => Weights.TryGetValue(action, out var weight) ? weight : 0m;
}
