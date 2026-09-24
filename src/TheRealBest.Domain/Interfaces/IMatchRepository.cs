namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Match?> GetByExternalIdAsync(string externalApiId, CancellationToken cancellationToken = default);
    Task<Match?> GetWithStatsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetByCompetitionAndSeasonAsync(Guid competitionId, int seasonYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MatchPlayerStats>> GetPlayerStatsByPlayerAndSeasonAsync(Guid playerId, int seasonYear, CancellationToken cancellationToken = default);
    Task<MatchPerformanceScore?> GetPerformanceScoreAsync(Guid matchId, Guid playerId, CancellationToken cancellationToken = default);
    Task AddAsync(Match match, CancellationToken cancellationToken = default);
    Task AddPlayerStatsAsync(MatchPlayerStats stats, CancellationToken cancellationToken = default);
    Task AddPerformanceScoreAsync(MatchPerformanceScore score, CancellationToken cancellationToken = default);
}