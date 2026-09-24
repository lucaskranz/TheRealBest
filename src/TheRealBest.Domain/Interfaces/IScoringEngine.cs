namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.ValueObjects;

public interface IScoringEngine
{
    int CurrentVersion { get; }

    MatchReceipt CalculateMatchScore(MatchPlayerStats stats, MatchContext context);

    /// <summary>
    /// Atalho para partidas carregadas do banco com Competition, HomeTeam e AwayTeam.
    /// </summary>
    MatchReceipt CalculateMatchScore(MatchPlayerStats stats, Match match) =>
        CalculateMatchScore(stats, MatchContext.From(match, stats.TeamId));

    /// <param name="totalSeasonMinutes">Minutos jogados na temporada, usados no FatorPresença.</param>
    SeasonScore CalculateSeasonScore(IEnumerable<MatchPerformanceScore> scores, int totalSeasonMinutes);
}
