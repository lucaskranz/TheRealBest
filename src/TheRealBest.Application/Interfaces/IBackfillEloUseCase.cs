namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs;

public interface IBackfillEloUseCase
{
    Task<EloBackfillSummaryDto> ExecuteAsync(int seasonYear, CancellationToken cancellationToken = default);
}
