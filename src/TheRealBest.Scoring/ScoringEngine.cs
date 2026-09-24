namespace TheRealBest.Scoring;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.Calculators;
using TheRealBest.Scoring.Rules;

/// <summary>
/// Implementação do Fair Player Index. Determinística: as mesmas estatísticas, contexto e regras
/// produzem sempre o mesmo recibo (o horário do cálculo vem do TimeProvider injetado).
/// </summary>
public sealed class ScoringEngine(IScoringRulesProvider rulesProvider, TimeProvider timeProvider) : IScoringEngine
{
    public int CurrentVersion => rulesProvider.GetRules().Version;

    public MatchReceipt CalculateMatchScore(MatchPlayerStats stats, MatchContext context) =>
        MatchPerformanceCalculator.Calculate(stats, context, rulesProvider.GetRules(), timeProvider.GetUtcNow().UtcDateTime);

    public SeasonScore CalculateSeasonScore(IEnumerable<MatchPerformanceScore> scores, int totalSeasonMinutes) =>
        SeasonScoreCalculator.Calculate(scores, totalSeasonMinutes);
}
