namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs.Comparison;

public interface IGetBallonDorComparisonUseCase
{
    /// <returns>Nulo quando não há classificação oficial cadastrada para o ano.</returns>
    Task<BallonDorComparisonDto?> ExecuteAsync(int year, CancellationToken cancellationToken = default);
}
