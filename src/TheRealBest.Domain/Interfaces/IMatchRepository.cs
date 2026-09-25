namespace TheRealBest.Domain.Interfaces;

using TheRealBest.Domain.Entities;

/// <param name="PlayerCount">Atuações gravadas (zero = a fonte não tinha estatísticas de jogador).</param>
/// <param name="TacticalPositionCount">
/// Atuações com posição tática além da letra da API (FB, CDM, CAM, W). Zero numa partida com jogadores indica escalação
/// sem grid: todos ficaram na posição padrão da letra (GK, CB, CM, ST).
/// </param>
public sealed record MatchImportAudit(
    string ExternalApiId,
    bool IsClubMatch,
    string HomeTeam,
    string AwayTeam,
    int? HomeEloRating,
    int? AwayEloRating,
    int PlayerCount,
    int TacticalPositionCount);

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Match?> GetByExternalIdAsync(string externalApiId, CancellationToken cancellationToken = default);
    /// <summary>Quais desses ids externos já têm partida gravada (com ou sem estatísticas de jogador).</summary>
    Task<IReadOnlySet<string>> GetExistingExternalIdsAsync(IReadOnlyCollection<string> externalApiIds, CancellationToken cancellationToken = default);
    Task<Match?> GetWithStatsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetByCompetitionAndSeasonAsync(Guid competitionId, int seasonYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MatchPlayerStats>> GetPlayerStatsByPlayerAndSeasonAsync(Guid playerId, int seasonYear, CancellationToken cancellationToken = default);
    Task<MatchPerformanceScore?> GetPerformanceScoreAsync(Guid matchId, Guid playerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MatchPerformanceScore>> GetSeasonScoresAsync(int seasonYear, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Match> Items, int TotalCount)> GetPagedMatchesAsync(int seasonYear, Guid? competitionId, Guid? teamId, int page, int pageSize, CancellationToken cancellationToken = default);
    /// <summary>Clube mais recente de cada jogador na temporada (ignora jogos de seleções).</summary>
    Task<IReadOnlyDictionary<Guid, Team>> GetLatestClubTeamsAsync(IReadOnlyCollection<Guid> playerIds, int seasonYear, CancellationToken cancellationToken = default);
    /// <summary>Partidas de clubes da temporada com o Elo de ao menos um dos times ausente.</summary>
    Task<IReadOnlyList<Guid>> GetClubMatchIdsMissingEloAsync(int seasonYear, CancellationToken cancellationToken = default);
    /// <summary>Partidas com competição, times, estatísticas e notas carregadas (rastreadas), para recalcular as notas.</summary>
    Task<IReadOnlyList<Match>> GetForRescoringAsync(IReadOnlyCollection<Guid> matchIds, CancellationToken cancellationToken = default);
    /// <summary>Resumo de conferência das partidas gravadas com esses ids externos.</summary>
    Task<IReadOnlyList<MatchImportAudit>> GetImportAuditAsync(IReadOnlyCollection<string> externalApiIds, CancellationToken cancellationToken = default);
    Task AddAsync(Match match, CancellationToken cancellationToken = default);
    Task AddPlayerStatsAsync(MatchPlayerStats stats, CancellationToken cancellationToken = default);
    Task AddPerformanceScoreAsync(MatchPerformanceScore score, CancellationToken cancellationToken = default);
}