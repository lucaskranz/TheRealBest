namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs;

public interface IRecalculateSeasonRankingUseCase
{
    Task<SeasonRankingSummaryDto> ExecuteAsync(int seasonYear, CancellationToken cancellationToken = default);
}