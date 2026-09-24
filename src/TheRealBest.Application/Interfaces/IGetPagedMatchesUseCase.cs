namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs.Common;
using TheRealBest.Application.DTOs.Matches;

public interface IGetPagedMatchesUseCase
{
    Task<PagedResult<MatchSummaryDto>> ExecuteAsync(MatchFilterDto filter, CancellationToken cancellationToken = default);
}