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
        var query = ApplyFilters(context.SeasonRankings.Include(r => r.Player), spec);

        // Com filtro de posição, a ordem é a da posição; senão, a geral
        query = spec.Position.HasValue
            ? query.OrderBy(r => r.PositionRank)
            : query.OrderBy(r => r.OverallRank);

        return await query
            .Skip(spec.Skip)
            .Take(spec.Take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountRankingsAsync(PlayerRankingSpec spec, CancellationToken cancellationToken = default) =>
        await ApplyFilters(context.SeasonRankings, spec).CountAsync(cancellationToken);

    /// <summary>Filtros comuns à listagem e à contagem: só elegíveis, posição, nacionalidade e busca por nome.</summary>
    private static IQueryable<SeasonRanking> ApplyFilters(IQueryable<SeasonRanking> query, PlayerRankingSpec spec)
    {
        query = query.Where(r => r.SeasonYear == spec.SeasonYear && r.IsRankingEligible);

        if (spec.Position.HasValue)
        {
            query = query.Where(r => r.Player.PrimaryPosition == spec.Position.Value);
        }

        if (!string.IsNullOrWhiteSpace(spec.Nationality))
        {
            query = query.Where(r => r.Player.Nationality == spec.Nationality);
        }

        if (!string.IsNullOrWhiteSpace(spec.Search))
        {
            // Sem diferenciar maiúsculas nem acentos; curingas digitados pelo usuário são tratados como texto
            var pattern = "%" + EscapeLike(spec.Search.Trim()) + "%";
            query = query.Where(r => EF.Functions.ILike(EF.Functions.Unaccent(r.Player.Name), EF.Functions.Unaccent(pattern)));
        }

        return query;
    }

    private static string EscapeLike(string term) =>
        term.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");

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