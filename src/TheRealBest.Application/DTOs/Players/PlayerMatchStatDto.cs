namespace TheRealBest.Application.DTOs.Players;

public sealed record PlayerMatchStatDto(
    Guid MatchId,
    DateTime MatchDate,
    string CompetitionName,
    string HomeTeamName,
    string AwayTeamName,
    int? HomeScore,
    int? AwayScore,
    int MinutesPlayed,
    int Goals,
    int Assists,
    int YellowCards,
    int RedCards,
    decimal? FinalMps
);
