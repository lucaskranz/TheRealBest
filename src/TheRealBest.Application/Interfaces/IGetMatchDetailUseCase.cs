namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs.Matches;

public interface IGetMatchDetailUseCase
{
    Task<MatchDetailDto?> ExecuteAsync(Guid matchId, CancellationToken cancellationToken = default);
}