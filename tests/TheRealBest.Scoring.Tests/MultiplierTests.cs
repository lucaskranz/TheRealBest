namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Domain.Enums;
using TheRealBest.Scoring.Multipliers;

public class MultiplierTests
{
    [Theory]
    [InlineData(CompetitionTier.WorldCup, true, "Final", 1.40)]
    [InlineData(CompetitionTier.WorldCup, false, "Group Stage - 1", 1.40)]
    [InlineData(CompetitionTier.UclKnockout, true, "Round of 16", 1.35)]
    [InlineData(CompetitionTier.UclKnockout, false, "League Stage - 3", 1.20)]
    [InlineData(CompetitionTier.InternationalContinental, false, "Group Stage - 2", 1.30)]
    [InlineData(CompetitionTier.TopLeague, false, "Regular Season - 5", 1.10)]
    [InlineData(CompetitionTier.DomesticCup, true, "Semi-finals", 1.05)]
    [InlineData(CompetitionTier.DomesticCup, true, "Final", 1.05)]
    [InlineData(CompetitionTier.DomesticCup, true, "Quarter-finals", 0.95)]
    [InlineData(CompetitionTier.DomesticCup, true, "3rd Round", 0.95)]
    [InlineData(CompetitionTier.OtherLeague, false, "Regular Season - 1", 0.95)]
    public void Tournament_ReturnsSpecificationWeight(CompetitionTier tier, bool isKnockout, string phase, decimal expected)
    {
        TournamentMultiplier.For(tier, isKnockout, phase).Should().Be(expected);
    }

    [Theory]
    [InlineData(2050, 1.20)]
    [InlineData(1880, 1.20)]
    [InlineData(1879, 1.10)]
    [InlineData(1780, 1.10)]
    [InlineData(1779, 1.00)]
    [InlineData(1600, 1.00)]
    [InlineData(1599, 0.90)]
    [InlineData(1500, 0.90)]
    public void OpponentStrength_UsesEloThresholds(int elo, decimal expected)
    {
        OpponentStrengthMultiplier.For(elo).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 0, 1.25)]
    [InlineData(2, 1, 1.25)]
    [InlineData(0, 1, 1.25)]
    [InlineData(2, 0, 1.00)]
    [InlineData(3, 0, 0.70)]
    [InlineData(6, 0, 0.70)]
    [InlineData(1, 5, 0.70)]
    public void Clutch_DependsOnFinalMargin(int teamScore, int opponentScore, decimal expected)
    {
        ClutchFactorMultiplier.ForFinalScore(teamScore, opponentScore).Should().Be(expected);
    }

    [Fact]
    public void Clutch_NegativeScore_Throws()
    {
        var act = () => ClutchFactorMultiplier.ForFinalScore(-1, 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
