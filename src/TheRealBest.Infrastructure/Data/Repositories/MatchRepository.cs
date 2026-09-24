namespace TheRealBest.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.Data;

public sealed class MatchRepository(AppDbContext context) : IMatchRepository
{
    public async Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Matches
            .Include(m => m.Competition)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<Match?> GetByExternalIdAsync(string externalApiId, CancellationToken cancellationToken = default) =>
        await context.Matches
            .Include(m => m.Competition)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .FirstOrDefaultAsync(m => m.ExternalApiId == externalApiId, cancellationToken);

    public async Task<Match?> GetWithStatsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Matches
            .Include(m => m.Competition)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.PlayerStats)
                .ThenInclude(ps => ps.Player)
            .Include(m => m.PerformanceScores)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Match>> GetByCompetitionAndSeasonAsync(Guid competitionId, int seasonYear, CancellationToken cancellationToken = default) =>
        await context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.CompetitionId == competitionId && m.Competition.SeasonYear == seasonYear)
            .OrderByDescending(m => m.MatchDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MatchPlayerStats>> GetPlayerStatsByPlayerAndSeasonAsync(Guid playerId, int seasonYear, CancellationToken cancellationToken = default) =>
        await context.MatchPlayerStats
            .Include(mps => mps.Match)
                .ThenInclude(m => m.Competition)
            .Include(mps => mps.Team)
            .Include(mps => mps.PerformanceScore)
            .Where(mps => mps.PlayerId == playerId && mps.Match.Competition.SeasonYear == seasonYear)
            .OrderByDescending(mps => mps.Match.MatchDate)
            .ToListAsync(cancellationToken);

    public async Task<MatchPerformanceScore?> GetPerformanceScoreAsync(Guid matchId, Guid playerId, CancellationToken cancellationToken = default) =>
        await context.MatchPerformanceScores
            .Include(s => s.MatchPlayerStats)
            .FirstOrDefaultAsync(s => s.MatchId == matchId && s.PlayerId == playerId, cancellationToken);

    public async Task<IReadOnlyList<MatchPerformanceScore>> GetSeasonScoresAsync(int seasonYear, CancellationToken cancellationToken = default) =>
        await context.MatchPerformanceScores
            .Include(s => s.Match)
                .ThenInclude(m => m.Competition)
            .Include(s => s.Player)
            .Include(s => s.MatchPlayerStats)
            .Where(s => s.Match.Competition.SeasonYear == seasonYear)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Match match, CancellationToken cancellationToken = default) =>
        await context.Matches.AddAsync(match, cancellationToken);

    public async Task AddPlayerStatsAsync(MatchPlayerStats stats, CancellationToken cancellationToken = default) =>
        await context.MatchPlayerStats.AddAsync(stats, cancellationToken);

    public async Task AddPerformanceScoreAsync(MatchPerformanceScore score, CancellationToken cancellationToken = default) =>
        await context.MatchPerformanceScores.AddAsync(score, cancellationToken);
}