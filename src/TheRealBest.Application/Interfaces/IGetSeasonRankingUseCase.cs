namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs.Common;
using TheRealBest.Application.DTOs.Ranking;

public interface IGetSeasonRankingUseCase
{
    Task<PagedResult<SeasonRankingItemDto>> ExecuteAsync(RankingFilterDto filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SeasonRankingItemDto>> GetTopContendersAsync(int seasonYear = 2023, int count = 5, CancellationToken cancellationToken = default);
}