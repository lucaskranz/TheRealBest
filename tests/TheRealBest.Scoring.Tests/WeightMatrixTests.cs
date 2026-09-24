namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Domain.Enums;
using TheRealBest.Scoring.Rules;
using TheRealBest.Scoring.WeightMatrices;

public class WeightMatrixTests
{
    private static readonly ScoringRuleSet Rules = ScoringRulesProvider.Default;

    [Fact]
    public void DefaultRules_EveryPosition_HasMatrixAndBaseline()
    {
        foreach (var position in Enum.GetValues<PlayerPosition>())
        {
            Rules.For(position).Weights.Should().NotBeEmpty();
            Rules.BaselineFor(position).Should().BePositive();
        }
    }

    [Fact]
    public void Goal_DefendersEarnMoreThanAttackers()
    {
        // Pesos invertidos por responsabilidade posicional: GK > CB > FB > CDM/CM > CAM/W > ST
        var goalWeights = new[] { PlayerPosition.GK, PlayerPosition.CB, PlayerPosition.FB, PlayerPosition.CDM, PlayerPosition.CAM, PlayerPosition.ST }
            .Select(p => Rules.For(p).WeightFor(ActionType.Goal));

        goalWeights.Should().BeInDescendingOrder().And.OnlyHaveUniqueItems();
    }

    [Theory]
    [InlineData(PlayerPosition.GK, 18)]
    [InlineData(PlayerPosition.CB, 14)]
    [InlineData(PlayerPosition.FB, 10)]
    [InlineData(PlayerPosition.CDM, 4)]
    [InlineData(PlayerPosition.CAM, 0)]
    [InlineData(PlayerPosition.W, 0)]
    [InlineData(PlayerPosition.ST, 0)]
    public void CleanSheet_IsHighForDefendersAndZeroForAttackers(PlayerPosition position, decimal expected)
    {
        Rules.For(position).WeightFor(ActionType.CleanSheet).Should().Be(expected);
    }

    [Theory]
    [InlineData(PlayerPosition.ST, -8)]
    [InlineData(PlayerPosition.CB, -2)]
    public void BigChanceMissed_IsSevereForStrikersAndLightForDefenders(PlayerPosition position, decimal expected)
    {
        Rules.For(position).WeightFor(ActionType.BigChanceMissed).Should().Be(expected);
    }

    [Fact]
    public void CentralMidfielder_UsesDefensiveMidfielderMatrix()
    {
        Rules.For(PlayerPosition.CM).Should().BeSameAs(Rules.For(PlayerPosition.CDM));
    }

    [Fact]
    public void Winger_MatchesAttackingMidfielderColumnOfSpecification()
    {
        WingerWeights.Matrix.Weights.Should().BeEquivalentTo(AttackingMidWeights.Matrix.Weights);
    }

    [Fact]
    public void AllWeights_HaveSignConsistentWithCatalog()
    {
        foreach (var position in Enum.GetValues<PlayerPosition>())
        {
            foreach (var (action, weight) in Rules.For(position).Weights)
            {
                ActionCatalog.Contains(action).Should().BeTrue($"{action} is weighted for {position}");

                var definition = ActionCatalog.Get(action);
                if (definition.IsPenalty)
                {
                    weight.Should().BeNegative($"{action} is a penalty for {position}");
                }
                else
                {
                    weight.Should().BePositive($"{action} is a positive action for {position}");
                }
            }
        }
    }

    [Fact]
    public void ToScoringWeights_ExportsEveryNonZeroWeight()
    {
        var exported = Rules.ToScoringWeights();

        var expectedCount = Enum.GetValues<PlayerPosition>().Sum(p => Rules.For(p).Weights.Count(w => w.Value != 0m));
        exported.Should().HaveCount(expectedCount);
        exported.Should().OnlyContain(w => w.IsPenalty == (w.WeightValue < 0m));
    }

    [Fact]
    public void FromWeights_RoundTripsDefaultRules()
    {
        var rebuilt = ScoringRuleSet.FromWeights(2, Rules.ToScoringWeights(), ScoringRulesProvider.DefaultBaselines);

        rebuilt.Version.Should().Be(2);
        foreach (var position in Enum.GetValues<PlayerPosition>())
        {
            rebuilt.For(position).Weights.Should().BeEquivalentTo(Rules.For(position).Weights);
        }
    }

    [Fact]
    public void ActionCatalog_KeysAreSnakeCase()
    {
        ActionCatalog.Get(ActionType.ExpectedGoalsOverperformance).Key.Should().Be("expected_goals_overperformance");
        ActionCatalog.Get(ActionType.Goal).Key.Should().Be("goal");
    }
}
