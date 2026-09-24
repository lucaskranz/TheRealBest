namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.Calculators;
using TheRealBest.Scoring.Rules;
using TheRealBest.Scoring.Tests.Builders;
using TheRealBest.Scoring.WeightMatrices;

public class MatchPerformanceCalculatorTests
{
    private static ScoringRuleSet RulesWithZeroBaseline() =>
        new(
            99,
            Enum.GetValues<PlayerPosition>().ToDictionary(p => p, p => ScoringRulesProvider.Default.For(p)),
            Enum.GetValues<PlayerPosition>().ToDictionary(p => p, _ => 0m));

    [Fact]
    public void Calculate_PlayerWithNoActionsAndZeroBaseline_Scores50InAnyContext()
    {
        var stats = StatsBuilder.For(PlayerPosition.CM).Build();
        var rules = RulesWithZeroBaseline();

        var contexts = new[]
        {
            Contexts.ChampionsLeagueKnockout(1, 0, Contexts.EloTop10),
            Contexts.League(6, 0, Contexts.EloWeak),
        };

        foreach (var context in contexts)
        {
            MatchPerformanceCalculator.Calculate(stats, context, rules, TestEngine.FixedNow).FinalMps.Should().Be(50m);
        }
    }

    [Fact]
    public void Calculate_ContextMultipliesPerformanceNotBaseScore()
    {
        var stats = StatsBuilder.For(PlayerPosition.CB).Tackles(3).Interceptions(2).Build();
        var rules = RulesWithZeroBaseline();

        var league = MatchPerformanceCalculator.Calculate(stats, Contexts.LeagueOpenGame(), rules, TestEngine.FixedNow);
        var final = MatchPerformanceCalculator.Calculate(
            stats, Contexts.ChampionsLeagueKnockout(1, 0, Contexts.EloTop10), rules, TestEngine.FixedNow);

        // Δ = 3×4 + 2×3.5 = 19
        league.FinalMps.Should().Be(50m + 19m * 1.10m);
        final.FinalMps.Should().Be(Math.Round(50m + 19m * final.ContextMultiplier.Combined, 2, MidpointRounding.AwayFromZero));
    }

    [Fact]
    public void Calculate_Receipt_IsInternallyConsistent()
    {
        var stats = StatsBuilder.For(PlayerPosition.CDM)
            .Minutes(75).Goals(1).Shots(2, 1).ExpectedGoals(0.3m).Passes(60, 54, progressive: 6)
            .Tackles(4).Recoveries(8).Cards(yellow: 1).Build();

        var receipt = TestEngine.Score(stats, Contexts.League(2, 1, Contexts.EloTop30));

        receipt.SubtotalRaw.Should().Be(receipt.ActionBreakdown.Concat(receipt.PenaltyBreakdown).Sum(i => i.TotalPoints));
        foreach (var item in receipt.ActionBreakdown.Concat(receipt.PenaltyBreakdown))
        {
            item.TotalPoints.Should().Be(Math.Round(item.Count * item.UnitWeight * item.MinutesFactor, 4, MidpointRounding.AwayFromZero));
        }

        var expected = Math.Round(Math.Clamp(
            receipt.BaseScore + (receipt.SubtotalRaw - receipt.PositionBaseline) * receipt.ContextMultiplier.Combined, 0m, 100m), 2, MidpointRounding.AwayFromZero);
        receipt.FinalMps.Should().Be(expected);

        receipt.ActionBreakdown.Should().OnlyContain(i => i.TotalPoints > 0);
        receipt.PenaltyBreakdown.Should().ContainSingle(i => i.ActionKey == "yellow_card" && i.TotalPoints == -3m);
        receipt.AlgorithmVersion.Should().Be(ScoringRulesProvider.DefaultVersion);
        receipt.CalculatedAt.Should().Be(TestEngine.FixedNow);
        receipt.PlayerId.Should().Be(stats.PlayerId);
        receipt.MatchId.Should().Be(stats.MatchId);
    }

    [Fact]
    public void Calculate_MinutesFactor_AppliesToVolumeActionsButNotToGoals()
    {
        // Entrou aos 60': 30 minutos → FatorMinutos = 0.3333
        var stats = StatsBuilder.For(PlayerPosition.ST).Minutes(30).Goals(1).Tackles(3).Build();

        var receipt = TestEngine.Score(stats, Contexts.LeagueOpenGame());

        receipt.MinutesFactor.Should().Be(0.3333m);
        receipt.ActionBreakdown.Should().ContainSingle(i => i.ActionKey == "goal")
            .Which.Should().Match<ActionScoreItem>(i => i.TotalPoints == 12m && i.MinutesFactor == 1m);
        receipt.ActionBreakdown.Should().ContainSingle(i => i.ActionKey == "tackle")
            .Which.TotalPoints.Should().Be(Math.Round(3 * 1.5m * 0.3333m, 4, MidpointRounding.AwayFromZero));
    }

    [Fact]
    public void Calculate_PositionBaseline_IsProportionalToMinutes()
    {
        var fullGame = TestEngine.Score(StatsBuilder.For(PlayerPosition.CB).Minutes(90).Build(), Contexts.LeagueOpenGame());
        var halfGame = TestEngine.Score(StatsBuilder.For(PlayerPosition.CB).Minutes(45).Build(), Contexts.LeagueOpenGame());

        fullGame.PositionBaseline.Should().Be(41m);
        halfGame.PositionBaseline.Should().Be(20.5m);
    }

    [Fact]
    public void Calculate_SplitsOpenPlayAndPenaltyGoals()
    {
        var stats = StatsBuilder.For(PlayerPosition.ST).Goals(2, penalties: 1).Build();

        var receipt = TestEngine.Score(stats, Contexts.LeagueOpenGame());

        receipt.ActionBreakdown.Should().ContainSingle(i => i.ActionKey == "goal" && i.Count == 1 && i.TotalPoints == 12m);
        receipt.ActionBreakdown.Should().ContainSingle(i => i.ActionKey == "penalty_goal" && i.Count == 1 && i.TotalPoints == 8m);
    }

    [Theory]
    [InlineData(19, 19, false)] // 100%, mas menos de 20 passes
    [InlineData(40, 34, false)] // 85% exatos não bastam
    [InlineData(40, 35, true)]  // 87.5%
    public void Calculate_PassAccuracyBonus_RequiresVolumeAndMoreThan85Percent(int total, int accurate, bool expectsBonus)
    {
        var stats = StatsBuilder.For(PlayerPosition.CM).Passes(total, accurate).Build();

        var receipt = TestEngine.Score(stats, Contexts.LeagueOpenGame());

        receipt.ActionBreakdown.Any(i => i.ActionKey == "pass_accuracy_bonus").Should().Be(expectsBonus);
    }

    [Theory]
    [InlineData(60, false)]
    [InlineData(61, true)]
    public void Calculate_CleanSheet_RequiresMoreThan60Minutes(int minutes, bool expectsCleanSheet)
    {
        var stats = StatsBuilder.For(PlayerPosition.CB).Minutes(minutes).CleanSheet().Build();

        var receipt = TestEngine.Score(stats, Contexts.League(1, 0, Contexts.EloMidTable));

        receipt.ActionBreakdown.Any(i => i.ActionKey == "clean_sheet").Should().Be(expectsCleanSheet);
    }

    [Fact]
    public void Calculate_GoalsConceded_PenalizesOnlyFromSecondGoal()
    {
        var stats = StatsBuilder.For(PlayerPosition.GK).GoalsConceded(3).Build();

        var receipt = TestEngine.Score(stats, Contexts.League(0, 3, Contexts.EloMidTable));

        receipt.PenaltyBreakdown.Should().ContainSingle(i => i.ActionKey == "goal_conceded")
            .Which.Should().Match<ActionScoreItem>(i => i.Count == 2 && i.TotalPoints == -8m);
    }

    [Fact]
    public void Calculate_GoalWithoutXgData_GetsNoOverperformanceBonus()
    {
        var withoutXg = TestEngine.Score(StatsBuilder.For(PlayerPosition.ST).Goals(1).Build(), Contexts.LeagueOpenGame());
        var withXg = TestEngine.Score(StatsBuilder.For(PlayerPosition.ST).Goals(1).ExpectedGoals(0.3m).Build(), Contexts.LeagueOpenGame());

        withoutXg.ActionBreakdown.Should().NotContain(i => i.ActionKey == "expected_goals_overperformance");
        withXg.ActionBreakdown.Should().ContainSingle(i => i.ActionKey == "expected_goals_overperformance")
            .Which.TotalPoints.Should().Be(4.9m);
    }

    [Fact]
    public void Calculate_OwnGoal_IsAFullDecisivePenalty()
    {
        var stats = StatsBuilder.For(PlayerPosition.CB).Minutes(10).OwnGoals(1).Build();

        var receipt = TestEngine.Score(stats, Contexts.League(0, 1, Contexts.EloMidTable));

        receipt.PenaltyBreakdown.Should().ContainSingle(i => i.ActionKey == "own_goal")
            .Which.Should().Match<ActionScoreItem>(i => i.TotalPoints == -16m && i.MinutesFactor == 1m);
        receipt.CountsTowardsSeason.Should().BeTrue();
    }

    [Fact]
    public void Calculate_ScoreIsClampedBetween0And100()
    {
        var outstanding = StatsBuilder.For(PlayerPosition.CB).Goals(3).Assists(2).CleanSheet().Build();
        var disastrous = StatsBuilder.For(PlayerPosition.CB).ErrorsLeadingToGoal(2).PenaltiesCommitted(1).Cards(red: 1).Build();

        TestEngine.Score(outstanding, Contexts.LeagueOpenGame(3, 0)).FinalMps.Should().Be(100m);
        TestEngine.Score(disastrous, Contexts.League(0, 3, Contexts.EloMidTable)).FinalMps.Should().Be(0m);
    }

    [Fact]
    public void Calculate_IsDeterministic()
    {
        var stats = StatsBuilder.For(PlayerPosition.W).Goals(1).Dribbles(6, 4).KeyPasses(3).ExpectedAssists(0.45m).Build();
        var context = Contexts.ChampionsLeagueKnockout(2, 1, Contexts.EloTop30, "Quarter-finals");

        var first = TestEngine.Score(stats, context);
        var second = TestEngine.Score(stats, context);

        second.Should().BeEquivalentTo(first);
    }
}
