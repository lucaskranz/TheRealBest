namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Scoring.Calculators;

public class MinutesFactorCalculatorTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10, 0.1111)]
    [InlineData(45, 0.5)]
    [InlineData(59, 0.6556)]
    [InlineData(60, 1.0)]
    [InlineData(75, 1.0)]
    [InlineData(90, 1.0)]
    [InlineData(105, 1.0833)]
    [InlineData(120, 1.1667)]
    public void Calculate_FollowsSpecificationPiecewiseFunction(int minutes, decimal expected)
    {
        MinutesFactorCalculator.Calculate(minutes).Should().Be(expected);
    }

    [Fact]
    public void Calculate_NegativeMinutes_Throws()
    {
        var act = () => MinutesFactorCalculator.Calculate(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
