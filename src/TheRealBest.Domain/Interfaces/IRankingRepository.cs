namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Specifications;

public interface IRankingRepository
{
    Task<SeasonRanking?> GetByPlayerAndSeasonAsync(Guid playerId, int seasonYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SeasonRanking>> GetRankingsAsync(PlayerRankingSpec spec, CancellationToken cancellationToken = default);
    /// <summary>Rankings da temporada (elegíveis ou não) dos jogadores com esses ids externos, com o jogador carregado.</summary>
    Task<IReadOnlyList<SeasonRanking>> GetByExternalPlayerIdsAsync(int seasonYear, IReadOnlyCollection<string> externalApiIds, CancellationToken cancellationToken = default);
    Task<int> CountRankingsAsync(PlayerRankingSpec spec, CancellationToken cancellationToken = default);
    Task UpsertRankingAsync(SeasonRanking ranking, CancellationToken cancellationToken = default);
    Task BulkUpsertRankingsAsync(IEnumerable<SeasonRanking> rankings, CancellationToken cancellationToken = default);
}