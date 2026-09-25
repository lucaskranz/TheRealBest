namespace TheRealBest.API.Commands;

using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <param name="Season">Ano de início da temporada (2023 = 2023/24).</param>
/// <param name="CompetitionId">Id da competição na API-Football; nulo = todas as do escopo da temporada.</param>
public sealed record SeasonToolRequest(int Season, int? CompetitionId);

/// <summary>
/// Comandos de manutenção de uma temporada já importada:
/// <c>--audit-season 2023 [--competition 39]</c> (relatório de conferência) e <c>--backfill-elo 2023</c> (Elo que faltou na importação).
/// </summary>
public static class SeasonToolsCommand
{
    public const string AuditFlag = "--audit-season";
    public const string BackfillEloFlag = "--backfill-elo";

    public static bool IsAuditRequested(string[] args) => args.Contains(AuditFlag);

    public static bool IsBackfillEloRequested(string[] args) => args.Contains(BackfillEloFlag);

    /// <returns>A requisição, ou nulo com <paramref name="error"/> preenchido quando os argumentos são inválidos.</returns>
    public static SeasonToolRequest? Parse(string[] args, string flag, out string? error)
    {
        error = null;

        if (!ImportSeasonCommand.TryReadInt(args, flag, required: true, out var season, ref error)
            || !ImportSeasonCommand.TryReadInt(args, ImportSeasonCommand.CompetitionFlag, required: false, out var competition, ref error))
        {
            return null;
        }

        if (!ApiFootballSeasonScope.IsSupported(season!.Value))
        {
            error = $"Season {season} is outside the supported range ({ApiFootballSeasonScope.FirstSeason}-{ApiFootballSeasonScope.CurrentSeason}).";
            return null;
        }

        if (competition is not null && ApiFootballSeasonScope.For(season.Value).All(e => e.LeagueId != competition))
        {
            error = $"Competition {competition} is not in the scope of season {season}.";
            return null;
        }

        return new SeasonToolRequest(season.Value, competition);
    }
}
