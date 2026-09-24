namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Team?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
}