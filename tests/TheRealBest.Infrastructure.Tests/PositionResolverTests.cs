namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using TheRealBest.Domain.Enums;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using static TheRealBest.Infrastructure.Tests.Support.ApiFootballFixtures;

public class PositionResolverTests
{
    [Theory]
    // City 3-2-4-1: Stones e Rodri à frente da linha de 3; Bernardo e Grealish abertos; De Bruyne e Gündoğan por dentro
    [InlineData("617", PlayerPosition.GK)]
    [InlineData("5", PlayerPosition.CB)]
    [InlineData("626", PlayerPosition.CDM)]
    [InlineData("44", PlayerPosition.CDM)]
    [InlineData("636", PlayerPosition.W)]
    [InlineData("629", PlayerPosition.CAM)]
    [InlineData("19187", PlayerPosition.W)]
    [InlineData("1100", PlayerPosition.ST)]
    [InlineData("631", PlayerPosition.CAM)] // Foden (M) entrou no lugar de De Bruyne
    [InlineData("627", PlayerPosition.CB)]  // Walker (D) entrou no lugar de Stones (volante): incompatível → padrão de "D"
    // Inter 3-5-2: Dumfries e Dimarco são alas; trio central de meio-campistas
    [InlineData("1836", PlayerPosition.CB)]
    [InlineData("226", PlayerPosition.FB)]
    [InlineData("31010", PlayerPosition.FB)]
    [InlineData("30558", PlayerPosition.CM)]
    [InlineData("217", PlayerPosition.ST)]
    [InlineData("907", PlayerPosition.ST)]   // Lukaku no lugar de Džeko
    [InlineData("91422", PlayerPosition.FB)] // Bellanova (D) no lugar do ala Dumfries
    public void UclFinal2023_ResolvesTacticalPositions(string playerId, PlayerPosition expected)
    {
        Position(UclFinal2023, playerId).Should().Be(expected);
    }

    [Theory]
    // Real 4-2-3-1: laterais nas pontas da defesa; Camavinga e Kroos volantes; Valverde e Rodrygo abertos atrás de Vinícius
    [InlineData("733", PlayerPosition.FB)]
    [InlineData("2285", PlayerPosition.CB)]
    [InlineData("2207", PlayerPosition.CDM)]
    [InlineData("756", PlayerPosition.W)]
    [InlineData("129718", PlayerPosition.CAM)]
    [InlineData("10009", PlayerPosition.W)]
    [InlineData("762", PlayerPosition.ST)]
    [InlineData("754", PlayerPosition.CDM)] // Modrić no lugar de Kroos
    [InlineData("372", PlayerPosition.FB)]  // Militão (D) no lugar do lateral Carvajal
    // City 4-1-4-1: Rodri volante isolado
    [InlineData("44", PlayerPosition.CDM)]
    [InlineData("1422", PlayerPosition.W)]  // Doku (F) no lugar do ponta Grealish
    public void UclQuarterFinal2024_ResolvesTacticalPositions(string playerId, PlayerPosition expected)
    {
        Position(UclQuarterFinal2024, playerId).Should().Be(expected);
    }

    [Fact]
    public void Resolve_WithoutLineups_FallsBackToApiLetter()
    {
        var positions = PositionResolver.Resolve(
            [],
            [],
            new Dictionary<int, string?> { [1] = "G", [2] = "D", [3] = "M", [4] = "F", [5] = null });

        positions.Should().BeEquivalentTo(new Dictionary<int, PlayerPosition>
        {
            [1] = PlayerPosition.GK,
            [2] = PlayerPosition.CB,
            [3] = PlayerPosition.CM,
            [4] = PlayerPosition.ST,
            [5] = PlayerPosition.CM,
        });
    }

    [Fact]
    public void Resolve_FourFourTwo_WideMidfieldersAreWingers()
    {
        var lineup = Lineup(
            (1, "G", "1:1"),
            (2, "D", "2:4"), (3, "D", "2:3"), (4, "D", "2:2"), (5, "D", "2:1"),
            (6, "M", "3:4"), (7, "M", "3:3"), (8, "M", "3:2"), (9, "M", "3:1"),
            (10, "F", "4:2"), (11, "F", "4:1"));

        var positions = PositionResolver.Resolve([lineup], [], new Dictionary<int, string?>());

        positions[2].Should().Be(PlayerPosition.FB);
        positions[3].Should().Be(PlayerPosition.CB);
        positions[6].Should().Be(PlayerPosition.W);
        positions[7].Should().Be(PlayerPosition.CM);
        positions[10].Should().Be(PlayerPosition.ST);
    }

    [Fact]
    public void Resolve_FourThreeThree_FrontThreeHasWingersAroundStriker()
    {
        var lineup = Lineup(
            (1, "G", "1:1"),
            (2, "D", "2:4"), (3, "D", "2:3"), (4, "D", "2:2"), (5, "D", "2:1"),
            (6, "M", "3:3"), (7, "M", "3:2"), (8, "M", "3:1"),
            (9, "F", "4:3"), (10, "F", "4:2"), (11, "F", "4:1"));

        var positions = PositionResolver.Resolve([lineup], [], new Dictionary<int, string?>());

        positions[7].Should().Be(PlayerPosition.CM);
        positions[9].Should().Be(PlayerPosition.W);
        positions[10].Should().Be(PlayerPosition.ST);
        positions[11].Should().Be(PlayerPosition.W);
    }

    private static PlayerPosition Position(string match, string playerId) =>
        Report(match).Performances.Single(p => p.Player.ExternalId == playerId).Stats.Position;

    private static LineupItem Lineup(params (int Id, string Pos, string Grid)[] players) =>
        new(
            new TeamInfo(99, "Team", null),
            Formation: null,
            players.Select(p => new LineupSlot(new LineupPlayer(p.Id, $"Player {p.Id}", p.Pos, p.Grid))).ToList(),
            Substitutes: []);
}
