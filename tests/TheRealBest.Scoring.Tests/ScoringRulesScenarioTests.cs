namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.Tests.Builders;

/// <summary>
/// Cenários exigidos em .agents/rules/scoring-rules.md.
/// </summary>
public class ScoringRulesScenarioTests
{
    [Fact]
    public void CenterBackWithCleanSheetAndFiveAerialDuels_BeatsStrikerWithOneJunkTimeGoal()
    {
        // Ambos partem de uma linha típica da posição; muda só o destaque de cada um
        var centerBack = TypicalLines.CenterBack().CleanSheet().GoalsConceded(0).AerialDuels(6, 5).Duels(9, 7).Build();
        var striker = TypicalLines.Striker().Goals(1).Shots(3, 2).ExpectedGoals(0.3m).Build();

        var centerBackMps = TestEngine.Score(centerBack, Contexts.League(1, 0, Contexts.EloMidTable)).FinalMps;
        var strikerMps = TestEngine.Score(striker, Contexts.League(5, 0, Contexts.EloWeak)).FinalMps;

        centerBackMps.Should().BeGreaterThan(strikerMps);
    }

    [Fact]
    public void LateWinnerAgainstTopRival_IsWorthMoreThanSameGoalInBlowoutAgainstWeakSide()
    {
        // Spec, seção 1: "Fazer o 5º gol numa goleada de 6 a 0 contra um adversário fraco não pode ter o mesmo
        // valor de marcar o gol da vitória aos 88 minutos contra um rival de topo."
        var oneGoal = TypicalLines.Striker().Goals(1).Shots(3, 2).ExpectedGoals(0.4m);

        var winner = TestEngine.Score(oneGoal.Build(), Contexts.League(1, 0, Contexts.EloTop10));
        var blowout = TestEngine.Score(oneGoal.Build(), Contexts.League(6, 0, Contexts.EloWeak));

        winner.FinalMps.Should().BeGreaterThan(blowout.FinalMps);
        winner.ContextMultiplier.Combined.Should().BeGreaterThan(2 * blowout.ContextMultiplier.Combined);
    }

    [Fact]
    public void SameGoal_EarnsMorePointsForCenterBackThanForStriker()
    {
        var context = Contexts.LeagueOpenGame();

        var centerBack = TestEngine.Score(StatsBuilder.For(PlayerPosition.CB).Goals(1).Build(), context);
        var striker = TestEngine.Score(StatsBuilder.For(PlayerPosition.ST).Goals(1).Build(), context);

        GoalPoints(centerBack).Should().BeGreaterThan(GoalPoints(striker));
    }

    [Fact]
    public void TypicalStatLines_ScoreCloseToEachOtherAcrossPositions()
    {
        // Sem a linha de base, volantes e zagueiros saturariam em 100 e atacantes ficariam bem abaixo.
        // As linhas típicas não têm gol/assistência (que entram na expectativa da posição), então ficam um pouco abaixo de 50.
        var typical = new Dictionary<PlayerPosition, MatchPlayerStats>
        {
            [PlayerPosition.GK] = TypicalLines.Goalkeeper().Build(),
            [PlayerPosition.CB] = TypicalLines.CenterBack().Build(),
            [PlayerPosition.FB] = TypicalLines.FullBack().Build(),
            [PlayerPosition.CDM] = TypicalLines.DefensiveMid().Build(),
            [PlayerPosition.W] = TypicalLines.Winger().Build(),
            [PlayerPosition.ST] = TypicalLines.Striker().Build(),
        };

        var scores = typical.ToDictionary(
            entry => entry.Key,
            entry => TestEngine.Score(entry.Value, Contexts.LeagueOpenGame(1, 1)).FinalMps);

        scores.Values.Should().OnlyContain(mps => mps >= 30m && mps <= 60m);
        (scores.Values.Max() - scores.Values.Min()).Should().BeLessThan(15m, "no position should be structurally favored");
    }

    [Fact]
    public void ZeroMinutes_ScoresNeutralAndDoesNotCountTowardsSeason()
    {
        var receipt = TestEngine.Score(StatsBuilder.For(PlayerPosition.CM).Minutes(0).Build(), Contexts.LeagueOpenGame());

        receipt.MinutesFactor.Should().Be(0m);
        receipt.PositionBaseline.Should().Be(0m);
        receipt.ActionBreakdown.Should().BeEmpty();
        receipt.PenaltyBreakdown.Should().BeEmpty();
        receipt.FinalMps.Should().Be(50m);
        receipt.CountsTowardsSeason.Should().BeFalse();
    }

    [Fact]
    public void ExtraTime_ScalesVolumeActionsAndBaselineAbove90Minutes()
    {
        var stats = StatsBuilder.For(PlayerPosition.CB).Minutes(120).Tackles(6).Build();

        var receipt = TestEngine.Score(stats, Contexts.ChampionsLeagueKnockout(1, 1, Contexts.EloTop10, "Quarter-finals"));

        receipt.MinutesFactor.Should().Be(1.1667m);
        receipt.ActionBreakdown.Single(i => i.ActionKey == "tackle").TotalPoints.Should().Be(28.0008m);
        receipt.PositionBaseline.Should().Be(47.8347m);
    }

    [Fact]
    public void RedCardInFirstMinute_IsFullyPenalizedAndCountsTowardsSeason()
    {
        var stats = StatsBuilder.For(PlayerPosition.CB).Minutes(1).Cards(red: 1).Build();

        var receipt = TestEngine.Score(stats, Contexts.League(0, 2, Contexts.EloMidTable));

        receipt.PenaltyBreakdown.Should().ContainSingle(i => i.ActionKey == "red_card")
            .Which.Should().Match<ActionScoreItem>(i => i.TotalPoints == -25m && i.MinutesFactor == 1m);
        receipt.FinalMps.Should().BeLessThan(25m);
        receipt.CountsTowardsSeason.Should().BeTrue();
    }

    [Fact]
    public void SuperSubWithDecisiveGoal_CountsTowardsSeasonWithFullGoalValue()
    {
        var stats = StatsBuilder.For(PlayerPosition.ST).Minutes(10).Goals(1).Shots(1, 1).ExpectedGoals(0.2m).Build();

        var receipt = TestEngine.Score(stats, Contexts.League(1, 0, Contexts.EloTop30));

        receipt.CountsTowardsSeason.Should().BeTrue();
        receipt.ActionBreakdown.Single(i => i.ActionKey == "goal").TotalPoints.Should().Be(12m);
        receipt.FinalMps.Should().BeGreaterThan(70m);
    }

    [Fact]
    public void ShortCameoWithoutDecisiveAction_DoesNotCountTowardsSeason()
    {
        var stats = StatsBuilder.For(PlayerPosition.CM).Minutes(15).Tackles(1).Passes(10, 9).Build();

        var receipt = TestEngine.Score(stats, Contexts.LeagueOpenGame());

        receipt.CountsTowardsSeason.Should().BeFalse();
    }

    [Fact]
    public void MatchOverload_RequiresLoadedNavigations()
    {
        IScoringEngine engine = TestEngine.Create();
        var match = new Match("ext-1", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Final", DateTime.UtcNow, true, 1, 0);
        var stats = StatsBuilder.For(PlayerPosition.CM).Build();

        var act = () => engine.CalculateMatchScore(stats, match);

        act.Should().Throw<InvalidOperationException>().WithMessage("*Competition, HomeTeam and AwayTeam*");
    }

    private static decimal GoalPoints(MatchReceipt receipt) =>
        receipt.ActionBreakdown.Single(i => i.ActionKey == "goal").TotalPoints;
}
