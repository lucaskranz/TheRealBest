namespace TheRealBest.Scoring.Tests.Builders;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;
using TheRealBest.Scoring.Rules;

public static class TestEngine
{
    public static readonly DateTime FixedNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public static ScoringEngine Create() => new(new ScoringRulesProvider(), new FixedTimeProvider(FixedNow));

    public static MatchReceipt Score(MatchPlayerStats stats, MatchContext context) =>
        Create().CalculateMatchScore(stats, context);

    public static MatchPerformanceScore StoredScore(
        decimal finalMps,
        decimal tournamentMultiplier = 1.10m,
        bool countsTowardsSeason = true) =>
        new(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PlayerPosition.CM,
            baseScore: 50m, actionBreakdownJson: "[]", penaltyBreakdownJson: "[]",
            subtotalRaw: 0m, positionBaseline: 0m,
            tournamentMultiplier, opponentMultiplier: 1m, clutchMultiplier: 1m,
            contextMultiplierCombined: tournamentMultiplier, minutesFactor: 1m,
            finalMps, algorithmVersion: 1, calculatedAt: FixedNow, countsTowardsSeason: countsTowardsSeason);

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utcNow);
    }
}
