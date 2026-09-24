namespace TheRealBest.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.Data;

public sealed class CompetitionRepository(AppDbContext context) : ICompetitionRepository
{
    public async Task<Competition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Competitions.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Competition?> GetByExternalIdAndSeasonAsync(string externalId, int seasonYear, CancellationToken cancellationToken = default) =>
        await context.Competitions.FirstOrDefaultAsync(c => c.ExternalApiId == externalId && c.SeasonYear == seasonYear, cancellationToken);

    public async Task AddAsync(Competition competition, CancellationToken cancellationToken = default) =>
        await context.Competitions.AddAsync(competition, cancellationToken);
}