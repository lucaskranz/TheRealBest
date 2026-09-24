namespace TheRealBest.Application.DTOs.Audit;

public sealed record MatchPerformanceReceiptDto(
    Guid ScoreId,
    Guid MatchId,
    Guid PlayerId,
    string PlayerName,
    string PlayerPosition,
    string? PhotoUrl,
    DateTime MatchDate,
    string CompetitionName,
    string HomeTeamName,
    string AwayTeamName,
    int? HomeScore,
    int? AwayScore,
    int MinutesPlayed,
    int AlgorithmVersion,
    DateTime CalculatedAt,
    AuditFormulaDto Formula,
    IReadOnlyList<AuditActionItemDto> PositiveActions,
    IReadOnlyList<AuditActionItemDto> Penalties
);