namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring;
using TheRealBest.Scoring.Rules;
using static TheRealBest.Infrastructure.Tests.Support.ApiFootballFixtures;

/// <summary>
/// Ponta a ponta com dados reais: resposta da API-Football → mapper → motor de pontuação.
/// </summary>
public class ApiFootballScoringTests
{
    private static readonly ScoringEngine Engine = new(new ScoringRulesProvider(), TimeProvider.System);

    [Fact]
    public void RodriUclFinal2023_TitleWinningGoal_IsEliteUnderCalibratedBaselines()
    {
        // Na v1 (linhas de base que assumiam métricas ausentes na API-Football) este jogo valia ~53.
        // Com a linha de base empírica da v2, o gol do título numa final fica no topo da escala.
        var report = Report(UclFinal2023);
        var rodri = report.Performances.Single(p => p.Player.ExternalId == "44");

        var receipt = Score(report, rodri, opponentElo: 1860);

        receipt.PositionEvaluated.Should().Be(PlayerPosition.CDM);
        receipt.SubtotalRaw.Should().Be(55.5m);
        receipt.PositionBaseline.Should().Be(31.2m);
        receipt.FinalMps.Should().BeGreaterThanOrEqualTo(90m);
    }

    [Theory]
    [InlineData(UclFinal2023)]
    [InlineData(UclQuarterFinal2024)]
    public void RealMatch_AveragePerformance_StaysCloseToNeutral(string match)
    {
        // Calibração v2: Σ(Δ − linha de base) por jogador deve ficar perto de zero numa partida real,
        // ou seja, a atuação média vale ~50 antes do contexto.
        var report = Report(match);

        var deviations = report.Performances
            .Where(p => p.Stats.MinutesPlayed >= 60)
            .Select(p => Score(report, p, opponentElo: null))
            .Select(r => r.SubtotalRaw - r.PositionBaseline)
            .ToList();

        deviations.Should().HaveCountGreaterThanOrEqualTo(20);
        deviations.Average().Should().BeInRange(-8m, 8m);
    }

    private static MatchReceipt Score(ExternalMatchReport report, ExternalPlayerPerformance performance, int? opponentElo)
    {
        var stats = MatchPlayerStats.FromStatLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), performance.Stats);
        var isHome = performance.TeamExternalId == report.Fixture.HomeTeam.ExternalId;
        var context = new MatchContext(
            report.Fixture.Competition.Tier,
            report.Fixture.IsKnockout,
            report.Fixture.RoundPhase,
            opponentElo,
            TeamScore: isHome ? report.Fixture.HomeScore!.Value : report.Fixture.AwayScore!.Value,
            OpponentScore: isHome ? report.Fixture.AwayScore!.Value : report.Fixture.HomeScore!.Value);

        return Engine.CalculateMatchScore(stats, context);
    }
}
