namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Player?> GetByExternalIdAsync(string externalApiId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetByPositionAsync(PlayerPosition position, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> SearchAsync(string query, int limit = 20, CancellationToken cancellationToken = default);
    Task AddAsync(Player player, CancellationToken cancellationToken = default);
    Task UpdateAsync(Player player, CancellationToken cancellationToken = default);
}