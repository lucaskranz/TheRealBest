namespace TheRealBest.Scoring.Rules;

using TheRealBest.Domain.Enums;
using TheRealBest.Scoring.WeightMatrices;

/// <summary>
/// Fonte das regras de pontuação usadas pelo motor. Permite trocar os pesos padrão (em código)
/// por pesos carregados do banco (scoring_weights) sem alterar o motor.
/// </summary>
public interface IScoringRulesProvider
{
    ScoringRuleSet GetRules();
}

/// <summary>
/// Regras padrão, definidas nas matrizes em código conforme docs/fair_ranking_formula_specification.md.
/// As versões anteriores continuam disponíveis para reproduzir scores históricos.
/// </summary>
public sealed class ScoringRulesProvider : IScoringRulesProvider
{
    public const int DefaultVersion = 2;

    private static readonly IReadOnlyDictionary<PlayerPosition, WeightMatrix> Matrices = new Dictionary<PlayerPosition, WeightMatrix>
    {
        [PlayerPosition.GK] = GoalkeeperWeights.Matrix,
        [PlayerPosition.CB] = CenterBackWeights.Matrix,
        [PlayerPosition.FB] = FullBackWeights.Matrix,
        [PlayerPosition.CDM] = DefensiveMidWeights.Matrix,
        [PlayerPosition.CM] = DefensiveMidWeights.Matrix,
        [PlayerPosition.CAM] = AttackingMidWeights.Matrix,
        [PlayerPosition.W] = WingerWeights.Matrix,
        [PlayerPosition.ST] = StrikerWeights.Matrix,
    };

    /// <summary>
    /// v1: linhas de base estimadas a partir de médias típicas por 90 minutos, assumindo métricas que a
    /// API-Football não fornece (recuperações, passes progressivos, duelos aéreos, xG).
    /// </summary>
    public static IReadOnlyDictionary<PlayerPosition, decimal> Version1Baselines { get; } = new Dictionary<PlayerPosition, decimal>
    {
        [PlayerPosition.GK] = 22m,
        [PlayerPosition.CB] = 41m,
        [PlayerPosition.FB] = 43m,
        [PlayerPosition.CDM] = 54m,
        [PlayerPosition.CM] = 54m,
        [PlayerPosition.CAM] = 44m,
        [PlayerPosition.W] = 44m,
        [PlayerPosition.ST] = 27m,
    };

    /// <summary>
    /// v2: média empírica de ΔAções por posição em 583 atuações completas (60–90 min) de dados reais da API-Football
    /// (29 partidas do ciclo 2023/24; seção 7.2 da especificação). Recalibrar quando a base crescer.
    /// </summary>
    public static IReadOnlyDictionary<PlayerPosition, decimal> DefaultBaselines { get; } = new Dictionary<PlayerPosition, decimal>
    {
        [PlayerPosition.GK] = 19.9m,
        [PlayerPosition.CB] = 20.1m,
        [PlayerPosition.FB] = 25.3m,
        [PlayerPosition.CDM] = 31.2m,
        [PlayerPosition.CM] = 26.0m,
        [PlayerPosition.CAM] = 35.1m,
        [PlayerPosition.W] = 28.8m,
        [PlayerPosition.ST] = 21.2m,
    };

    public static ScoringRuleSet Version1 { get; } = new(1, Matrices, Version1Baselines);

    public static ScoringRuleSet Default { get; } = new(DefaultVersion, Matrices, DefaultBaselines);

    public ScoringRuleSet GetRules() => Default;
}
