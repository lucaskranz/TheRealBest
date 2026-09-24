namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs.Audit;

public interface IGetMatchAuditReceiptUseCase
{
    Task<MatchPerformanceReceiptDto?> ExecuteAsync(Guid matchId, Guid playerId, CancellationToken cancellationToken = default);
}