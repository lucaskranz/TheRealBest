namespace TheRealBest.Application.DTOs.Audit;

public sealed record AuditFormulaDto(
    decimal BaseScore,
    decimal SubtotalRaw,
    decimal PositionBaseline,
    decimal MinutesFactor,
    decimal TournamentMultiplier,
    decimal OpponentMultiplier,
    decimal ClutchMultiplier,
    decimal ContextMultiplierCombined,
    decimal FinalMps,
    bool CountsTowardsSeason
);