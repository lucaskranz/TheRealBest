namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs;
using TheRealBest.Domain.Ingestion;

public interface IIngestMatchDataUseCase
{
    Task<IngestionResultDto> ExecuteAsync(ExternalMatchReport report, CancellationToken cancellationToken = default);
}