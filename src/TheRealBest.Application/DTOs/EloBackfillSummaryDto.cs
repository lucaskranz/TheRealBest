namespace TheRealBest.Application.DTOs;

/// <param name="MatchesMissingElo">Partidas de clubes da temporada sem o Elo de ao menos um dos times, antes da execução.</param>
/// <param name="MatchesUpdated">Partidas que ganharam ao menos um rating nesta execução (e tiveram as notas recalculadas).</param>
/// <param name="PerformancesRescored">Notas recalculadas.</param>
/// <param name="TeamsNotFound">Clubes que continuam sem rating: fonte indisponível na data ou nome sem correspondência no ClubElo.</param>
public sealed record EloBackfillSummaryDto(
    int SeasonYear,
    int MatchesMissingElo,
    int MatchesUpdated,
    int PerformancesRescored,
    IReadOnlyList<string> TeamsNotFound);
