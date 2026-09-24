namespace TheRealBest.Domain.Tests;

using FluentAssertions;
using TheRealBest.Domain.ValueObjects;
using Xunit;

public class ContextMultiplierTests
{
    [Fact]
    public void Combined_ShouldCalculateProductOfAllMultipliers()
    {
        // Arrange
        var tournament = 1.35m; // UCL Knockout
        var opponent = 1.20m;   // Top 10 ELO
        var clutch = 1.25m;     // Clutch time

        // Act
        var multiplier = new ContextMultiplier(tournament, opponent, clutch);

        // Assert
        // 1.35 * 1.20 * 1.25 = 2.025
        multiplier.Combined.Should().Be(2.025m);
    }

    [Fact]
    public void Default_ShouldReturnOneForEveryComponent()
    {
        // Act
        var def = ContextMultiplier.Default;

        // Assert
        def.TournamentMultiplier.Should().Be(1.0m);
        def.OpponentMultiplier.Should().Be(1.0m);
        def.ClutchMultiplier.Should().Be(1.0m);
        def.Combined.Should().Be(1.0m);
    }
}