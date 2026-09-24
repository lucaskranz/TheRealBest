namespace TheRealBest.Scoring.Tests.Builders;

using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// Contextos de partida prontos. Elo na escala ClubElo: 1950 = top 10, 1820 = top 30, 1700 = meio de tabela, 1550 = fraco.
/// </summary>
public static class Contexts
{
    public const int EloTop10 = 1950;
    public const int EloTop30 = 1820;
    public const int EloMidTable = 1700;
    public const int EloWeak = 1550;

    /// <summary>
    /// Rodada de liga top 5 contra meio de tabela com 2 gols de diferença: MultContexto = 1.10 × 1.00 × 1.00 = 1.10.
    /// </summary>
    public static MatchContext LeagueOpenGame(int teamScore = 2, int opponentScore = 0) =>
        League(teamScore, opponentScore, EloMidTable);

    public static MatchContext League(int teamScore, int opponentScore, int opponentElo) =>
        new(CompetitionTier.TopLeague, IsKnockout: false, RoundPhase: "Regular Season - 10", opponentElo, teamScore, opponentScore);

    public static MatchContext ChampionsLeagueKnockout(int teamScore, int opponentScore, int opponentElo, string phase = "Final") =>
        new(CompetitionTier.UclKnockout, IsKnockout: true, phase, opponentElo, teamScore, opponentScore);
}
