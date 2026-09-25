namespace TheRealBest.API.Formula;

using TheRealBest.Application.Localization;
using TheRealBest.Domain.Enums;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Scoring.Calculators;
using TheRealBest.Scoring.Multipliers;
using TheRealBest.Scoring.Rules;

public sealed class FormulaDescriptorFactory(IScoringRulesProvider rulesProvider, ActionLabelResolver labels)
{
    public FormulaDescriptor Create(string locale)
    {
        var rules = rulesProvider.GetRules();
        var positions = Enum.GetValues<PlayerPosition>();

        return new FormulaDescriptor(
            rules.Version,
            new MpsRules(MatchPerformanceCalculator.BaseScore, MatchPerformanceCalculator.MinScore, MatchPerformanceCalculator.MaxScore),
            new MinutesRules(
                MinutesFactorCalculator.FullFactorFromMinute,
                MinutesFactorCalculator.RegulationMinutes,
                MinutesFactorCalculator.ExtraTimeDivisor,
                MatchPerformanceCalculator.MinMinutesForSeason),
            positions
                .Select(p => new FormulaPosition(
                    p.ToString(),
                    labels.PositionLabel(p.ToString(), locale),
                    rules.BaselineFor(p),
                    SharedMatrixOwner(rules, p, positions)))
                .ToList(),
            ActionCatalog.All
                .Select(a => new FormulaAction(
                    a.Key,
                    labels.ActionLabel(a.Key, 1m, locale),
                    a.Category.ToString(),
                    a.IsPenalty,
                    a.ScalesWithMinutes,
                    a.IsDecisive,
                    HasDataSource(a.Type),
                    positions
                        .Select(p => (code: p.ToString(), weight: rules.For(p).WeightFor(a.Type)))
                        .Where(x => x.weight != 0m)
                        .ToDictionary(x => x.code, x => x.weight)))
                .Where(a => a.Weights.Count > 0)
                .ToList(),
            [
                new("worldCup", TournamentMultiplier.WorldCup),
                new("uclKnockout", TournamentMultiplier.ChampionsLeagueKnockout),
                new("continentalNationalTeams", TournamentMultiplier.ContinentalNationalTeams),
                new("uclLeaguePhase", TournamentMultiplier.ChampionsLeagueLeaguePhase),
                new("topLeague", TournamentMultiplier.TopLeague),
                new("domesticCupLateStage", TournamentMultiplier.DomesticCupLateStage),
                new("other", TournamentMultiplier.Other),
            ],
            [
                new("top10", OpponentStrengthMultiplier.Top10EloThreshold, OpponentStrengthMultiplier.Top10),
                new("top30", OpponentStrengthMultiplier.Top30EloThreshold, OpponentStrengthMultiplier.Top30),
                new("midTable", OpponentStrengthMultiplier.MidTableEloThreshold, OpponentStrengthMultiplier.MidTable),
                new("weak", null, OpponentStrengthMultiplier.Weak),
                new("unrated", null, OpponentStrengthMultiplier.For(null)),
            ],
            [
                new("tight", 0, ClutchFactorMultiplier.TightMaxMargin, ClutchFactorMultiplier.Tight),
                new("open", ClutchFactorMultiplier.TightMaxMargin + 1, ClutchFactorMultiplier.JunkTimeMinMargin - 1, ClutchFactorMultiplier.Open),
                new("junkTime", ClutchFactorMultiplier.JunkTimeMinMargin, null, ClutchFactorMultiplier.JunkTime),
            ],
            new SeasonRules(
                SeasonScoreCalculator.FullPresenceMinutes,
                SeasonScoreCalculator.MinMatchesForRanking,
                SeasonScoreCalculator.MinMinutesForRanking));
    }

    private static bool HasDataSource(ActionType action) =>
        !StatsActionMapper.UnmappedActions.Contains(action) && !ApiFootballMapper.UnavailableActions.Contains(action);

    /// <summary>Primeira posição anterior que usa exatamente a mesma matriz (ex.: CM → CDM).</summary>
    private static string? SharedMatrixOwner(ScoringRuleSet rules, PlayerPosition position, PlayerPosition[] all) =>
        all.TakeWhile(p => p != position)
            .Where(p => ReferenceEquals(rules.For(p), rules.For(position)))
            .Select(p => p.ToString())
            .FirstOrDefault();
}
