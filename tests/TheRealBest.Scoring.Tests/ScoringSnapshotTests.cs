namespace TheRealBest.Scoring.Tests;

using FluentAssertions;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.Rules;
using TheRealBest.Scoring.Tests.Builders;

/// <summary>
/// Snapshots do algoritmo v1. Se um destes testes quebrar, o cálculo mudou: scores históricos seriam alterados.
/// Mudanças intencionais exigem incrementar ScoringRulesProvider.DefaultVersion e registrar os novos valores.
/// </summary>
public class ScoringSnapshotTests
{
    public static TheoryData<string, decimal, decimal, decimal, decimal> Snapshots => new()
    {
        // cenário,                        SubtotalRaw, LinhaDeBase, MultContexto, MPS
        { "rodri-ucl-final-2023",          91.9m,       54m,         1.8563m,      100m },
        { "vinicius-ucl-final-2024",       69.7m,       44m,         1.485m,       88.16m },
        { "haaland-5-goals-leipzig-2023",  95.9m,       27m,         1.0395m,      100m },
        { "rudiger-vs-city-2024",          67.552m,     47.8347m,    2.025m,       89.93m },
        { "typical-cdm-league-draw",       47.5m,       54m,         1.375m,       41.06m },
        { "typical-st-league-draw",        18m,         27m,         1.375m,       37.63m },
    };

    [Fact]
    public void AlgorithmVersion_IsOne()
    {
        ScoringRulesProvider.DefaultVersion.Should().Be(1, "snapshots below were recorded for v1");
    }

    [Theory]
    [MemberData(nameof(Snapshots))]
    public void Score_MatchesRecordedSnapshot(string scenario, decimal subtotal, decimal baseline, decimal multiplier, decimal mps)
    {
        var (stats, context) = Scenario(scenario);

        var receipt = TestEngine.Score(stats, context);

        receipt.SubtotalRaw.Should().Be(subtotal);
        receipt.PositionBaseline.Should().Be(baseline);
        receipt.ContextMultiplier.Combined.Should().Be(multiplier);
        receipt.FinalMps.Should().Be(mps);
    }

    private static (MatchPlayerStats, MatchContext) Scenario(string name) => name switch
    {
        "rodri-ucl-final-2023" => RealMatches.RodriUclFinal2023(),
        "vinicius-ucl-final-2024" => RealMatches.ViniciusUclFinal2024(),
        "haaland-5-goals-leipzig-2023" => RealMatches.HaalandFiveGoalsVsLeipzig2023(),
        "rudiger-vs-city-2024" => RealMatches.RudigerVsManCity2024(),
        "typical-cdm-league-draw" => (TypicalLines.DefensiveMid().Build(), Contexts.LeagueOpenGame(1, 1)),
        "typical-st-league-draw" => (TypicalLines.Striker().Build(), Contexts.LeagueOpenGame(1, 1)),
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
    };
}
