namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs.Players;
using TheRealBest.Domain.Enums;

public interface ISearchPlayersUseCase
{
    Task<IReadOnlyList<PlayerSummaryDto>> ExecuteAsync(string? query, PlayerPosition? position, int limit = 20, CancellationToken cancellationToken = default);
}