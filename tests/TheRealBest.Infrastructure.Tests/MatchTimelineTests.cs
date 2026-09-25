namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Infrastructure.Tests.Support;

public class MatchTimelineTests
{
    private const int Home = 10;
    private const int Away = 20;
    private const int HomeDefender = 101;
    private const int HomeStriker = 102;
    private const int HomeSub = 103;
    private const int AwayStriker = 201;

    private static readonly HashSet<int> Starters = [HomeDefender, HomeStriker, AwayStriker];

    [Fact]
    public void OwnGoal_AttributedToScorerTeam_BenefitsOpponent()
    {
        // Zagueiro da casa faz gol contra; evento registrado no time dele. Placar final 0 × 1.
        var timeline = MatchTimeline.Build(
            [Events.Goal(30, Home, HomeDefender, "Own Goal")], Starters, Home, Away, homeScore: 0, awayScore: 1);

        timeline.MatchesFinalScore.Should().BeTrue();
        timeline.OwnGoalsBy(HomeDefender).Should().Be(1);
        timeline.GoalsConcededWhileOnPitch(HomeStriker, Home).Should().Be(1);
        timeline.GoalsConcededWhileOnPitch(AwayStriker, Away).Should().Be(0);
    }

    [Fact]
    public void OwnGoal_AttributedToBeneficiaryTeam_IsDetectedFromFinalScore()
    {
        // Mesmo lance, mas a fonte registra o evento no time beneficiado
        var timeline = MatchTimeline.Build(
            [Events.Goal(30, Away, HomeDefender, "Own Goal")], Starters, Home, Away, homeScore: 0, awayScore: 1);

        timeline.MatchesFinalScore.Should().BeTrue();
        timeline.GoalsConcededWhileOnPitch(HomeStriker, Home).Should().Be(1);
        timeline.GoalsConcededWhileOnPitch(AwayStriker, Away).Should().Be(0);
    }

    [Fact]
    public void VarCancelledGoal_IsRemoved()
    {
        var timeline = MatchTimeline.Build(
            [Events.Goal(40, Away, AwayStriker), Events.VarGoalCancelled(41, Away, AwayStriker)],
            Starters, Home, Away, homeScore: 0, awayScore: 0);

        timeline.MatchesFinalScore.Should().BeTrue();
        timeline.GoalsConcededWhileOnPitch(HomeDefender, Home).Should().Be(0);
    }

    [Fact]
    public void VarCancelledGoal_NotListedAsGoal_KeepsEarlierValidGoal()
    {
        // Chelsea 1 × 1 Liverpool (2023/24): gol válido aos 18', gol anulado aos 30' que a fonte não lista como "Goal"
        var timeline = MatchTimeline.Build(
            [Events.Goal(18, Away, AwayStriker), Events.VarGoalCancelled(30, Away, AwayStriker), Events.Goal(37, Home, HomeStriker)],
            Starters, Home, Away, homeScore: 1, awayScore: 1);

        timeline.MatchesFinalScore.Should().BeTrue();
        timeline.GoalsConcededWhileOnPitch(HomeDefender, Home).Should().Be(1);
        timeline.GoalsConcededWhileOnPitch(AwayStriker, Away).Should().Be(1);
    }

    [Fact]
    public void Substitution_WithSwappedPlayers_IsCorrected()
    {
        // Evento invertido: "player" é quem entra e "assist" quem sai (o titular)
        var timeline = MatchTimeline.Build(
            [Events.Substitution(60, Home, outId: HomeSub, inId: HomeStriker), Events.Goal(70, Away, AwayStriker)],
            Starters, Home, Away, homeScore: 0, awayScore: 1);

        timeline.Substitutions.Should().ContainSingle()
            .Which.Should().Match<MatchTimeline.Substitution>(s => s.PlayerInId == HomeSub && s.PlayerOutId == HomeStriker);
        timeline.GoalsConcededWhileOnPitch(HomeStriker, Home).Should().Be(0);
        timeline.GoalsConcededWhileOnPitch(HomeSub, Home).Should().Be(1);
    }

    [Fact]
    public void FirstHalfStoppageTimeGoal_HappensBeforeSecondHalfSubstitution()
    {
        // Gol aos 45+2 e substituição no intervalo (46'): quem entrou não estava em campo no gol
        var timeline = MatchTimeline.Build(
            [Events.Goal(45, Away, AwayStriker, extra: 2), Events.Substitution(46, Home, HomeStriker, HomeSub)],
            Starters, Home, Away, homeScore: 0, awayScore: 1);

        timeline.GoalsConcededWhileOnPitch(HomeSub, Home).Should().Be(0);
        timeline.GoalsConcededWhileOnPitch(HomeStriker, Home).Should().Be(1);
    }

    [Fact]
    public void SentOffPlayer_DoesNotConcedeAfterLeaving()
    {
        var timeline = MatchTimeline.Build(
            [Events.RedCard(20, Home, HomeDefender), Events.Goal(50, Away, AwayStriker)],
            Starters, Home, Away, homeScore: 0, awayScore: 1);

        timeline.GoalsConcededWhileOnPitch(HomeDefender, Home).Should().Be(0);
        timeline.GoalsConcededWhileOnPitch(HomeStriker, Home).Should().Be(1);
    }

    [Fact]
    public void EventsInconsistentWithFinalScore_AreFlagged()
    {
        var timeline = MatchTimeline.Build([Events.Goal(10, Home, HomeStriker)], Starters, Home, Away, homeScore: 2, awayScore: 0);

        timeline.MatchesFinalScore.Should().BeFalse();
    }
}
