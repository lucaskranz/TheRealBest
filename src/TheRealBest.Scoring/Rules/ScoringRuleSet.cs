namespace TheRealBest.Scoring.Rules;

using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.WeightMatrices;

/// <summary>
/// Conjunto versionado de matrizes de peso e linhas de base por posição. Toda alteração exige nova versão,
/// para que scores já calculados continuem rastreáveis via algorithm_version.
/// </summary>
public sealed class ScoringRuleSet
{
    private readonly IReadOnlyDictionary<PlayerPosition, WeightMatrix> _matrices;
    private readonly IReadOnlyDictionary<PlayerPosition, decimal> _baselines;

    /// <param name="baselines">
    /// Δ esperado de uma atuação média de 90 minutos em cada posição. É descontado do Δ do jogador
    /// (proporcional aos minutos), de modo que uma atuação média vale 50 em qualquer posição.
    /// </param>
    public ScoringRuleSet(
        int version,
        IReadOnlyDictionary<PlayerPosition, WeightMatrix> matrices,
        IReadOnlyDictionary<PlayerPosition, decimal> baselines)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);
        EnsureAllPositions(matrices, nameof(matrices));
        EnsureAllPositions(baselines, nameof(baselines));

        Version = version;
        _matrices = matrices;
        _baselines = baselines;
    }

    public int Version { get; }

    public WeightMatrix For(PlayerPosition position) => _matrices[position];

    public decimal BaselineFor(PlayerPosition position) => _baselines[position];

    /// <summary>
    /// Todos os pesos no formato do domínio, por exemplo para popular a tabela scoring_weights.
    /// </summary>
    public IReadOnlyList<ScoringWeight> ToScoringWeights() =>
        Enum.GetValues<PlayerPosition>()
            .SelectMany(position => ActionCatalog.All
                .Select(action => (action, weight: For(position).WeightFor(action.Type)))
                .Where(x => x.weight != 0m)
                .Select(x => new ScoringWeight(position, x.action.Type, x.weight, x.action.IsPenalty, x.action.Description)))
            .ToList();

    /// <summary>
    /// Monta um conjunto de regras a partir de pesos externos (ex.: carregados do banco).
    /// Posições sem nenhum peso ficam com matriz vazia.
    /// </summary>
    public static ScoringRuleSet FromWeights(
        int version,
        IEnumerable<ScoringWeight> weights,
        IReadOnlyDictionary<PlayerPosition, decimal> baselines)
    {
        var byPosition = weights
            .GroupBy(w => w.Position)
            .ToDictionary(g => g.Key, g => g.ToDictionary(w => w.ActionType, w => w.WeightValue));

        var matrices = Enum.GetValues<PlayerPosition>().ToDictionary(
            position => position,
            position => new WeightMatrix(
                byPosition.TryGetValue(position, out var map) ? map : new Dictionary<ActionType, decimal>()));

        return new ScoringRuleSet(version, matrices, baselines);
    }

    private static void EnsureAllPositions<T>(IReadOnlyDictionary<PlayerPosition, T> byPosition, string paramName)
    {
        var missing = Enum.GetValues<PlayerPosition>().Where(p => !byPosition.ContainsKey(p)).ToList();
        if (missing.Count > 0)
        {
            throw new ArgumentException($"Missing values for positions: {string.Join(", ", missing)}.", paramName);
        }
    }
}
