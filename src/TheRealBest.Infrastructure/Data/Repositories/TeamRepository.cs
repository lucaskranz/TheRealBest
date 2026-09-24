namespace TheRealBest.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.Data;

public sealed class TeamRepository(AppDbContext context) : ITeamRepository
{
    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Teams.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<Team?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
        await context.Teams.FirstOrDefaultAsync(t => t.ExternalApiId == externalId, cancellationToken);

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default) =>
        await context.Teams.AddAsync(team, cancellationToken);
}