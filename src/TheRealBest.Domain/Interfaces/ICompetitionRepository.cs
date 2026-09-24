namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;

public interface ICompetitionRepository
{
    Task<Competition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Competition?> GetByExternalIdAndSeasonAsync(string externalId, int seasonYear, CancellationToken cancellationToken = default);
    Task AddAsync(Competition competition, CancellationToken cancellationToken = default);
}