namespace TheRealBest.Application.Tests;

using System.Text.Json;
using FluentAssertions;
using TheRealBest.Application.Players;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;
using Xunit;
using static TheRealBest.Application.Players.PlayerAttributeCalculator;

public class PlayerAttributeCalculatorTests
{
    [Fact]
    public void Calculate_ConvertsCategoryPointsToPer90AndPercentileAmongSamePosition()
    {
        // 5 zagueiros com 900 min; defesa: 10, 20, 30, 40 e 50 pontos. O alvo (40) supera ou empata 4 de 5 = 80%.
        var target = Guid.NewGuid();
        var samples = new List<ScoreSample> { Sample(target, PlayerPosition.CB, 900, defending: 40) };
        samples.AddRange(new[] { 10m, 20m, 30m, 50m }.Select(points => Sample(Guid.NewGuid(), PlayerPosition.CB, 900, defending: points)));
        // Atacante com muita defesa não entra na comparação de zagueiros
        samples.Add(Sample(Guid.NewGuid(), PlayerPosition.ST, 900, defending: 500));

        var attributes = Calculate(target, PlayerPosition.CB, samples);

        var defending = attributes.Single(a => a.Category == "Defending");
        defending.Per90.Should().Be(4m); // 40 pontos em 900 min
        defending.Percentile.Should().Be(80);
        defending.PeerCount.Should().Be(5);
    }

    [Fact]
    public void Calculate_ComparesPositionsThatShareTheWeightMatrix()
    {
        // Volante (CDM) é comparado com meio-campistas (CM): mesma matriz de pesos no motor
        var target = Guid.NewGuid();
        var samples = new List<ScoreSample> { Sample(target, PlayerPosition.CDM, 900, defending: 40) };
        samples.AddRange(Enumerable.Range(0, 4).Select(_ => Sample(Guid.NewGuid(), PlayerPosition.CM, 900, defending: 10)));

        var defending = Calculate(target, PlayerPosition.CDM, samples).Single(a => a.Category == "Defending");

        defending.PeerCount.Should().Be(5);
        defending.Percentile.Should().Be(100);
    }

    [Fact]
    public void Calculate_FewPeers_OmitsPercentileButKeepsPer90()
    {
        var target = Guid.NewGuid();

        var attributes = Calculate(target, PlayerPosition.CB, [Sample(target, PlayerPosition.CB, 900, defending: 40)]);

        attributes.Should().OnlyContain(a => a.Percentile == null);
        attributes.Single(a => a.Category == "Defending").Per90.Should().Be(4m);
    }

    [Fact]
    public void Calculate_PeersNeedMinimumMinutes()
    {
        var target = Guid.NewGuid();
        var samples = new List<ScoreSample> { Sample(target, PlayerPosition.CB, 900, defending: 40) };
        samples.AddRange(Enumerable.Range(0, 6).Select(_ => Sample(Guid.NewGuid(), PlayerPosition.CB, MinMinutesForComparison - 1, defending: 1)));

        var attributes = Calculate(target, PlayerPosition.CB, samples);

        attributes.Single(a => a.Category == "Defending").PeerCount.Should().Be(1, "only the target reaches the minutes threshold");
    }

    [Fact]
    public void Calculate_UsesGoalkeeperAxesForGoalkeepers()
    {
        var target = Guid.NewGuid();

        var attributes = Calculate(target, PlayerPosition.GK, [Sample(target, PlayerPosition.GK, 900, defending: 0)]);

        attributes.Select(a => a.Category).Should().Contain("Goalkeeping").And.NotContain("Finishing");
    }

    [Fact]
    public void Calculate_UnknownPlayer_ReturnsEmpty()
    {
        Calculate(Guid.NewGuid(), PlayerPosition.CB, Array.Empty<ScoreSample>()).Should().BeEmpty();
    }

    private static ScoreSample Sample(Guid playerId, PlayerPosition position, int minutes, decimal defending)
    {
        var items = new[] { new ActionScoreItem("tackle", "tackle", 1, defending, defending, Category: ActionCategory.Defending) };
        return new ScoreSample(playerId, position, minutes, JsonSerializer.Serialize(items), "[]");
    }
}
