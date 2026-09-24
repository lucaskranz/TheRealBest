namespace TheRealBest.Application.DTOs;

public sealed record IngestionResultDto(
    Guid MatchId,
    string ExternalFixtureId,
    int PlayersProcessed,
    int ScoresCalculated,
    bool AlreadyExists = false
);