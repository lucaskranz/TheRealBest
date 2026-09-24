namespace TheRealBest.Application.Interfaces;

using TheRealBest.Application.DTOs;

public interface IMatchDataIngestionPipeline
{
    Task<IReadOnlyList<IngestionResultDto>> IngestCompetitionSeasonAsync(
        string competitionExternalId,
        int seasonYear,
        CancellationToken cancellationToken = default);
}