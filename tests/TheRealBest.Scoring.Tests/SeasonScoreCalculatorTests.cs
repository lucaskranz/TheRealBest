namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.Calculators;
using TheRealBest.Scoring.Tests.Builders;

public class SeasonScoreCalculatorTests
{
    [Fact]
    public void Calculate_QualityBeatsVolume_AndSmallSamplesAreDiscounted()
    {
        // Spec, seção 1: um jogador mediano com muitas partidas não pode superar um craque com 35 partidas magistrais,
        // e 8 jogos perfeitos antes de uma lesão não podem vencer a temporada.
        var star = Season(matches: 35, mps: 70m, minutes: 3000);
        var average = Season(matches: 60, mps: 50m, minutes: 5000);
        var injured = Season(matches: 8, mps: 85m, minutes: 700);

        star.Fss.Should().Be(70m);
        average.Fss.Should().Be(50m);
        injured.Fss.Should().BeLessThan(average.Fss);
        star.Fss.Should().BeGreaterThan(average.Fss);
    }

    [Theory]
    [InlineData(10, 900, true)]
    [InlineData(35, 3000, true)]
    [InlineData(9, 2000, false)]  // minutos suficientes, poucas partidas
    [InlineData(10, 899, false)]  // partidas suficientes, poucos minutos
    [InlineData(8, 700, false)]   // o "lesionado" dos 8 jogos perfeitos
    public void Calculate_RankingEligibility_RequiresTenMatchesAnd900Minutes(int matches, int minutes, bool expected)
    {
        Season(matches, mps: 70m, minutes).IsRankingEligible.Should().Be(expected);
    }

    [Fact]
    public void Calculate_RankingEligibility_IgnoresMatchesThatDoNotCountTowardsSeason()
    {
        // 9 partidas válidas + 3 participações curtas sem ação decisiva = 9 partidas para o ranking
        var scores = Enumerable.Range(0, 9).Select(_ => TestEngine.StoredScore(70m))
            .Concat(Enumerable.Range(0, 3).Select(_ => TestEngine.StoredScore(50m, countsTowardsSeason: false)));

        var result = SeasonScoreCalculator.Calculate(scores, totalSeasonMinutes: 1000);

        result.MatchesCounted.Should().Be(9);
        result.IsRankingEligible.Should().BeFalse();
    }

    [Fact]
    public void Calculate_WeightsAverageByTournament()
    {
        var scores = new[]
        {
            TestEngine.StoredScore(80m, tournamentMultiplier: 1.35m),
            TestEngine.StoredScore(60m, tournamentMultiplier: 1.10m),
        };

        var result = SeasonScoreCalculator.Calculate(scores, totalSeasonMinutes: 2200);

        // (80 × 1.35 + 60 × 1.10) / (1.35 + 1.10) = 174 / 2.45
        result.WeightedMpsSum.Should().Be(174m);
        result.WeightedMpsAverage.Should().Be(71.0204m);
        result.MpsAverage.Should().Be(70m);
        result.Fss.Should().Be(71.0204m);
    }

    [Fact]
    public void Calculate_IgnoresMatchesThatDoNotCountTowardsSeason()
    {
        var scores = new[]
        {
            TestEngine.StoredScore(80m),
            TestEngine.StoredScore(50m, countsTowardsSeason: false),
        };

        var result = SeasonScoreCalculator.Calculate(scores, totalSeasonMinutes: 2200);

        result.MatchesCounted.Should().Be(1);
        result.Fss.Should().Be(80m);
    }

    [Fact]
    public void Calculate_NoCountedMatches_ReturnsEmpty()
    {
        var result = SeasonScoreCalculator.Calculate([TestEngine.StoredScore(90m, countsTowardsSeason: false)], 10);

        result.Should().Be(SeasonScore.Empty);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(550, 0.5)]
    [InlineData(700, 0.5641)]
    [InlineData(2200, 1.0)]
    [InlineData(4000, 1.0)]
    public void PresenceFactor_IsSquareRootOfMinutesShareCappedAtOne(int minutes, decimal expected)
    {
        SeasonScoreCalculator.CalculatePresenceFactor(minutes).Should().Be(expected);
    }

    [Fact]
    public void Engine_DelegatesSeasonScore()
    {
        var result = TestEngine.Create().CalculateSeasonScore([TestEngine.StoredScore(64m)], 2200);

        result.Fss.Should().Be(64m);
    }

    private static SeasonScore Season(int matches, decimal mps, int minutes) =>
        SeasonScoreCalculator.Calculate(
            Enumerable.Range(0, matches).Select(_ => TestEngine.StoredScore(mps)),
            minutes);
}
