namespace TheRealBest.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.Data;

public sealed class PlayerRepository(AppDbContext context) : IPlayerRepository
{
    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Players
            .Include(p => p.MatchStats)
            .Include(p => p.PerformanceScores)
            .Include(p => p.SeasonRankings)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Player?> GetByExternalIdAsync(string externalApiId, CancellationToken cancellationToken = default) =>
        await context.Players
            .FirstOrDefaultAsync(p => p.ExternalApiId == externalApiId, cancellationToken);

    public async Task<IReadOnlyList<Player>> GetByPositionAsync(PlayerPosition position, CancellationToken cancellationToken = default) =>
        await context.Players
            .Where(p => p.PrimaryPosition == position)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Player>> SearchAsync(string query, int limit = 20, CancellationToken cancellationToken = default) =>
        await context.Players
            .Where(p => EF.Functions.ILike(p.Name, $"%{query}%") || EF.Functions.ILike(p.Nationality, $"%{query}%"))
            .OrderBy(p => p.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Player player, CancellationToken cancellationToken = default) =>
        await context.Players.AddAsync(player, cancellationToken);

    public Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        context.Players.Update(player);
        return Task.CompletedTask;
    }
}