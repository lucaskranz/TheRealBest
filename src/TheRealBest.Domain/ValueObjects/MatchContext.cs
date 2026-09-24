namespace TheRealBest.Domain.ValueObjects;

using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;

/// <summary>
/// Contexto de uma partida do ponto de vista do time do jogador, usado pelo motor de pontuação.
/// OpponentEloRanking é o rating do adversário na data do jogo; nulo quando indisponível (multiplicador neutro).
/// </summary>
public sealed record MatchContext(
    CompetitionTier Tier,
    bool IsKnockout,
    string RoundPhase,
    int? OpponentEloRanking,
    int TeamScore,
    int OpponentScore
)
{
    /// <summary>
    /// Monta o contexto a partir de uma partida com Competition, HomeTeam e AwayTeam carregados.
    /// </summary>
    public static MatchContext From(Match match, Guid teamId)
    {
        if (match.Competition is null || match.HomeTeam is null || match.AwayTeam is null)
        {
            throw new InvalidOperationException(
                $"Match {match.Id} must be loaded with Competition, HomeTeam and AwayTeam to be scored.");
        }

        if (match.HomeScore is null || match.AwayScore is null)
        {
            throw new InvalidOperationException($"Match {match.Id} has no final score and cannot be scored.");
        }

        var isHome = teamId == match.HomeTeamId;
        if (!isHome && teamId != match.AwayTeamId)
        {
            throw new ArgumentException($"Team {teamId} did not play match {match.Id}.", nameof(teamId));
        }

        return new MatchContext(
            match.Competition.Tier,
            match.IsKnockout,
            match.RoundPhase,
            OpponentEloRanking: isHome ? match.AwayEloRating : match.HomeEloRating,
            TeamScore: isHome ? match.HomeScore.Value : match.AwayScore.Value,
            OpponentScore: isHome ? match.AwayScore.Value : match.HomeScore.Value);
    }
}
