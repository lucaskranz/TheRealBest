namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Scoring.Tests.Builders;

/// <summary>
/// Atuações reais marcantes devem pontuar bem acima de uma atuação comum.
/// Estatísticas aproximadas: ver <see cref="RealMatches"/>.
/// </summary>
public class RealMatchScenarioTests
{
    [Fact]
    public void Rodri_TitleWinningGoalInUclFinal_IsElite()
    {
        var (stats, context) = RealMatches.RodriUclFinal2023();

        TestEngine.Score(stats, context).FinalMps.Should().BeGreaterThanOrEqualTo(90m);
    }

    [Fact]
    public void Vinicius_GoalInUclFinal_IsElite()
    {
        var (stats, context) = RealMatches.ViniciusUclFinal2024();

        TestEngine.Score(stats, context).FinalMps.Should().BeGreaterThanOrEqualTo(80m);
    }

    [Fact]
    public void Haaland_FiveGoalsInBlowout_HitsTheCeilingDespiteJunkTimeDiscount()
    {
        var (stats, context) = RealMatches.HaalandFiveGoalsVsLeipzig2023();

        var receipt = TestEngine.Score(stats, context);

        receipt.ContextMultiplier.ClutchMultiplier.Should().Be(0.70m);
        receipt.ActionBreakdown.Should().Contain(i => i.ActionKey == "penalty_goal" && i.Count == 1);
        receipt.FinalMps.Should().Be(100m);
    }

    [Fact]
    public void Rudiger_ExtraTimeDefensiveShiftAgainstCity_IsHighlyRated()
    {
        var (stats, context) = RealMatches.RudigerVsManCity2024();

        var receipt = TestEngine.Score(stats, context);

        receipt.MinutesFactor.Should().BeGreaterThan(1m);
        receipt.FinalMps.Should().BeGreaterThanOrEqualTo(80m);
    }

    [Fact]
    public void Rudiger_DefendingWithoutScoring_OutscoresOrdinaryStrikerGoal()
    {
        var (rudiger, rudigerContext) = RealMatches.RudigerVsManCity2024();
        var ordinaryGoal = TypicalLines.Striker().Goals(1).Shots(3, 2).ExpectedGoals(0.4m).Build();

        var defender = TestEngine.Score(rudiger, rudigerContext).FinalMps;
        var striker = TestEngine.Score(ordinaryGoal, Contexts.LeagueOpenGame(3, 1)).FinalMps;

        defender.Should().BeGreaterThan(striker);
    }
}
