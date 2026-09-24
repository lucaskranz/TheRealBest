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
/// Regras padrão (v1), definidas nas matrizes em código conforme docs/fair_ranking_formula_specification.md.
/// </summary>
public sealed class ScoringRulesProvider : IScoringRulesProvider
{
    public const int DefaultVersion = 1;

    /// <summary>
    /// Linhas de base provisórias da v1: Δ de uma atuação média por 90 minutos, estimado aplicando médias
    /// típicas de titulares das 5 grandes ligas às matrizes de peso (derivação na seção 7 da especificação).
    /// Devem ser recalibradas com dados reais (etapa 3C), o que gera uma nova versão do algoritmo.
    /// </summary>
    public static IReadOnlyDictionary<PlayerPosition, decimal> DefaultBaselines { get; } = new Dictionary<PlayerPosition, decimal>
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

    public static ScoringRuleSet Default { get; } = new(
        DefaultVersion,
        new Dictionary<PlayerPosition, WeightMatrix>
        {
            [PlayerPosition.GK] = GoalkeeperWeights.Matrix,
            [PlayerPosition.CB] = CenterBackWeights.Matrix,
            [PlayerPosition.FB] = FullBackWeights.Matrix,
            [PlayerPosition.CDM] = DefensiveMidWeights.Matrix,
            [PlayerPosition.CM] = DefensiveMidWeights.Matrix,
            [PlayerPosition.CAM] = AttackingMidWeights.Matrix,
            [PlayerPosition.W] = WingerWeights.Matrix,
            [PlayerPosition.ST] = StrikerWeights.Matrix,
        },
        DefaultBaselines);

    public ScoringRuleSet GetRules() => Default;
}
