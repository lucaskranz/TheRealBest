namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring;
using TheRealBest.Scoring.Rules;
using static TheRealBest.Infrastructure.Tests.Support.ApiFootballFixtures;

/// <summary>
/// Ponta a ponta com dados reais: resposta da API-Football → mapper → motor de pontuação.
/// </summary>
public class ApiFootballScoringTests
{
    [Fact]
    public void RodriUclFinal2023_WithApiFootballDataOnly_ScoresBarelyAboveNeutral()
    {
        // Marcador de calibração: as linhas de base v1 assumem métricas que a API-Football não fornece
        // (recuperações, passes progressivos, duelos aéreos, xG). O gol do título fica pouco acima de 50.
        // A recalibração da Etapa 3C deve mover este valor; atualize o intervalo junto com a nova versão do algoritmo.
        var report = Report(UclFinal2023);
        var rodri = report.Performances.Single(p => p.Player.ExternalId == "44");
        var stats = MatchPlayerStats.FromStatLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), rodri.Stats);
        var context = new MatchContext(
            report.Fixture.Competition.Tier, report.Fixture.IsKnockout, report.Fixture.RoundPhase,
            OpponentEloRanking: 1860, report.Fixture.HomeScore!.Value, report.Fixture.AwayScore!.Value);

        var receipt = new ScoringEngine(new ScoringRulesProvider(), TimeProvider.System).CalculateMatchScore(stats, context);

        receipt.PositionEvaluated.Should().Be(PlayerPosition.CDM);
        receipt.SubtotalRaw.Should().Be(55.5m);
        receipt.FinalMps.Should().BeInRange(50m, 56m);
        receipt.ActionBreakdown.Should().NotContain(i => i.ActionKey == "expected_goals_overperformance");
    }
}
