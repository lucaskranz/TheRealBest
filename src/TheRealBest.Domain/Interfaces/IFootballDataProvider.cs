namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;

public interface IFootballDataProvider
{
    Task<IReadOnlyList<Match>> FetchRecentMatchesAsync(int seasonYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MatchPlayerStats>> FetchMatchPlayerStatsAsync(string matchExternalId, CancellationToken cancellationToken = default);
    Task<Player?> FetchPlayerDetailsAsync(string playerExternalId, CancellationToken cancellationToken = default);
}