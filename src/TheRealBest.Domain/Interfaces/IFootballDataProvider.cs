namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Ingestion;

/// <summary>
/// Fonte externa de partidas e estatísticas por jogador.
/// </summary>
public interface IFootballDataProvider
{
    /// <summary>Partidas de uma competição numa temporada (ex.: season 2023 = temporada 2023/24).</summary>
    Task<IReadOnlyList<ExternalFixture>> GetFixturesAsync(
        string competitionExternalId,
        int seasonYear,
        CancellationToken cancellationToken = default);

    /// <summary>Estatísticas de todos os jogadores que entraram em campo numa partida encerrada.</summary>
    Task<ExternalMatchReport> GetMatchReportAsync(ExternalFixture fixture, CancellationToken cancellationToken = default);
}
