namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.ValueObjects;

public interface IScoringEngine
{
    int CurrentVersion { get; }
    MatchReceipt CalculateMatchScore(MatchPlayerStats stats, Match match);
    decimal CalculateSeasonScore(IEnumerable<MatchPerformanceScore> scores, int totalSeasonMinutes);
}