namespace TheRealBest.Scoring.Calculators;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.Multipliers;
using TheRealBest.Scoring.Rules;

/// <summary>
/// Calcula o Match Performance Score (MPS):
/// MPS = Clamp(0, 100, Base + (Σ pontos das ações − LinhaDeBase_posição × FatorMinutos) × MultContexto),
/// onde os pontos das ações de volume já vêm multiplicados pelo FatorMinutos e
/// MultContexto = W_torneio × W_adversário × W_clutch.
/// </summary>
/// <remarks>
/// Decisões de implementação da v1 (seção 7 de docs/fair_ranking_formula_specification.md):
/// o contexto multiplica apenas o desempenho, não a base 50; a linha de base da posição faz a atuação média valer 50;
/// e o FatorMinutos incide só sobre ações de volume (um gol ou um vermelho valem inteiros para quem entrou tarde).
/// </remarks>
public static class MatchPerformanceCalculator
{
    public const decimal BaseScore = 50.0m;
    public const decimal MinScore = 0m;
    public const decimal MaxScore = 100m;

    /// <summary>Abaixo disso, a partida só conta no FSS se houver ação decisiva no placar.</summary>
    public const int MinMinutesForSeason = 20;

    public static MatchReceipt Calculate(
        MatchPlayerStats stats,
        MatchContext context,
        ScoringRuleSet rules,
        DateTime calculatedAt)
    {
        var matrix = rules.For(stats.PositionPlayed);
        var minutesFactor = MinutesFactorCalculator.Calculate(stats.MinutesPlayed);
        var counts = StatsActionMapper.Map(stats);

        var actions = new List<ActionScoreItem>();
        var penalties = new List<ActionScoreItem>();
        var hasDecisiveAction = false;

        foreach (var action in ActionCatalog.All)
        {
            if (!counts.TryGetValue(action.Type, out var count) || count == 0m)
            {
                continue;
            }

            hasDecisiveAction |= action.IsDecisive;

            var weight = matrix.WeightFor(action.Type);
            if (weight == 0m)
            {
                continue;
            }

            var itemFactor = action.ScalesWithMinutes ? minutesFactor : 1.0m;
            var item = new ActionScoreItem(
                action.Key,
                Label: action.Key,
                count,
                weight,
                TotalPoints: ScoringMath.Round4(count * weight * itemFactor),
                MinutesFactor: itemFactor,
                Category: action.Category);

            (action.IsPenalty ? penalties : actions).Add(item);
        }

        var subtotal = actions.Concat(penalties).Sum(i => i.TotalPoints);
        var positionBaseline = ScoringMath.Round4(rules.BaselineFor(stats.PositionPlayed) * minutesFactor);
        var contextMultiplier = new ContextMultiplier(
            TournamentMultiplier.For(context.Tier, context.IsKnockout, context.RoundPhase),
            OpponentStrengthMultiplier.For(context.OpponentEloRanking),
            ClutchFactorMultiplier.ForFinalScore(context.TeamScore, context.OpponentScore));

        var finalMps = ScoringMath.Round2(Math.Clamp(
            BaseScore + (subtotal - positionBaseline) * contextMultiplier.Combined,
            MinScore,
            MaxScore));

        return new MatchReceipt(
            stats.PlayerId,
            stats.MatchId,
            stats.PositionPlayed,
            BaseScore,
            actions,
            penalties,
            subtotal,
            positionBaseline,
            contextMultiplier,
            minutesFactor,
            finalMps,
            rules.Version,
            calculatedAt,
            CountsTowardsSeason: stats.MinutesPlayed >= MinMinutesForSeason || hasDecisiveAction);
    }
}
