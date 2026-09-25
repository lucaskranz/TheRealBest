namespace TheRealBest.Domain.Tests;

using FluentAssertions;
using TheRealBest.Domain.Entities;
using Xunit;

public class SeasonRankingTests
{
    [Fact]
    public void RecalculateFrom_UpdatesValuesButKeepsIdentity()
    {
        var playerId = Guid.NewGuid();
        var stored = new SeasonRanking(playerId, 2023, 10, 900, 55m, 600m, 0.64m, 35m, 3, 1, 1.1m, isRankingEligible: true);
        var recalculated = new SeasonRanking(playerId, 2023, 12, 1100, 60m, 720m, 0.71m, 42.6m, 2, 1, 1.2m, isRankingEligible: true, "[1]");
        var originalId = stored.Id;
        var originalCreatedAt = stored.CreatedAt;

        stored.RecalculateFrom(recalculated);

        stored.Id.Should().Be(originalId);
        stored.CreatedAt.Should().Be(originalCreatedAt);
        stored.Should().BeEquivalentTo(recalculated, o => o
            .Excluding(r => r.Id)
            .Excluding(r => r.CreatedAt)
            .Excluding(r => r.UpdatedAt)
            .Excluding(r => r.Player));
        stored.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void RecalculateFrom_OtherPlayer_Throws()
    {
        var stored = new SeasonRanking(Guid.NewGuid(), 2023, 10, 900, 55m, 600m, 0.64m, 35m, 3, 1, 1.1m, isRankingEligible: true);
        var other = new SeasonRanking(Guid.NewGuid(), 2023, 10, 900, 55m, 600m, 0.64m, 35m, 3, 1, 1.1m, isRankingEligible: true);

        var act = () => stored.RecalculateFrom(other);

        act.Should().Throw<ArgumentException>();
    }
}
