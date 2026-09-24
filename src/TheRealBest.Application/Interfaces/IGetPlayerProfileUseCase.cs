namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs.Players;

public interface IGetPlayerProfileUseCase
{
    Task<PlayerProfileDto?> ExecuteAsync(Guid playerId, int seasonYear = 2023, CancellationToken cancellationToken = default);
}