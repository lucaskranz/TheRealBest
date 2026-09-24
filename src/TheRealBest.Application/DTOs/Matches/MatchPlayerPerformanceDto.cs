namespace TheRealBest.Application.DTOs.Matches;

public sealed record MatchPlayerPerformanceDto(
    Guid PlayerId,
    string PlayerName,
    string? PhotoUrl,
    string PositionPlayed,
    string PositionPlayedLabel,
    Guid TeamId,
    string TeamName,
    int MinutesPlayed,
    int Goals,
    int Assists,
    int YellowCards,
    int RedCards,
    decimal? FinalMps,
    Guid? ScoreId
);