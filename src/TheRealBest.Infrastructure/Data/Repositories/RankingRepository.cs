namespace TheRealBest.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.Specifications;
using TheRealBest.Infrastructure.Data;

public sealed class RankingRepository(AppDbContext context) : IRankingRepository
{
    public async Task<SeasonRanking?> GetByPlayerAndSeasonAsync(Guid playerId, int seasonYear, CancellationToken cancellationToken = default) =>
        await context.SeasonRankings
            .Include(r => r.Player)
            .FirstOrDefaultAsync(r => r.PlayerId == playerId && r.SeasonYear == seasonYear, cancellationToken);

    public async Task<IReadOnlyList<SeasonRanking>> GetRankingsAsync(PlayerRankingSpec spec, CancellationToken cancellationToken = default)
    {
        var query = context.SeasonRankings
            .Include(r => r.Player)
            .Where(r => r.SeasonYear == spec.SeasonYear && r.IsRankingEligible);

        if (spec.Position.HasValue)
        {
            query = query.Where(r => r.Player.PrimaryPosition == spec.Position.Value);
        }

        if (!string.IsNullOrWhiteSpace(spec.Nationality))
        {
            query = query.Where(r => r.Player.Nationality == spec.Nationality);
        }

        if (spec.Position.HasValue)
        {
            query = query.OrderBy(r => r.PositionRank);
        }
        else
        {
            query = query.OrderBy(r => r.OverallRank);
        }

        return await query
            .Skip(spec.Skip)
            .Take(spec.Take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountRankingsAsync(PlayerRankingSpec spec, CancellationToken cancellationToken = default)
    {
        var query = context.SeasonRankings
            .Where(r => r.SeasonYear == spec.SeasonYear && r.IsRankingEligible);

        if (spec.Position.HasValue)
        {
            query = query.Where(r => r.Player.PrimaryPosition == spec.Position.Value);
        }

        if (!string.IsNullOrWhiteSpace(spec.Nationality))
        {
            query = query.Where(r => r.Player.Nationality == spec.Nationality);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task UpsertRankingAsync(SeasonRanking ranking, CancellationToken cancellationToken = default)
    {
        var existing = await context.SeasonRankings
            .FirstOrDefaultAsync(r => r.PlayerId == ranking.PlayerId && r.SeasonYear == ranking.SeasonYear, cancellationToken);

        if (existing is null)
        {
            await context.SeasonRankings.AddAsync(ranking, cancellationToken);
        }
        else
        {
            context.Entry(existing).CurrentValues.SetValues(ranking);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task BulkUpsertRankingsAsync(IEnumerable<SeasonRanking> rankings, CancellationToken cancellationToken = default)
    {
        var rankingList = rankings.ToList();
        if (rankingList.Count == 0) return;

        var seasonYears = rankingList.Select(r => r.SeasonYear).Distinct().ToList();
        var playerIds = rankingList.Select(r => r.PlayerId).Distinct().ToList();

        var existingList = await context.SeasonRankings
            .Where(r => seasonYears.Contains(r.SeasonYear) && playerIds.Contains(r.PlayerId))
            .ToListAsync(cancellationToken);

        var existingMap = existingList.ToDictionary(r => (r.PlayerId, r.SeasonYear));

        foreach (var ranking in rankingList)
        {
            if (existingMap.TryGetValue((ranking.PlayerId, ranking.SeasonYear), out var existing))
            {
                context.Entry(existing).CurrentValues.SetValues(ranking);
            }
            else
            {
                await context.SeasonRankings.AddAsync(ranking, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}