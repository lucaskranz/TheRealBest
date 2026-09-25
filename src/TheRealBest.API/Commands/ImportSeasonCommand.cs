namespace TheRealBest.API.Commands;

using System.Globalization;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;
using TheRealBest.Infrastructure.Ingestion;

/// <summary>
/// Linha de comando da importação de temporada:
/// <c>dotnet run --project src/TheRealBest.API -- --import-season 2023 [--competition 9] [--limit 5]</c>.
/// </summary>
public static class ImportSeasonCommand
{
    public const string Flag = "--import-season";
    public const string CompetitionFlag = "--competition";
    public const string LimitFlag = "--limit";

    public static bool IsRequested(string[] args) => args.Contains(Flag);

    /// <returns>A requisição, ou nulo com <paramref name="error"/> preenchido quando os argumentos são inválidos.</returns>
    public static SeasonImportRequest? Parse(string[] args, out string? error)
    {
        error = null;

        if (!TryReadInt(args, Flag, required: true, out var season, ref error))
        {
            return null;
        }

        if (!ApiFootballSeasonScope.IsSupported(season!.Value))
        {
            error = $"Season {season} is outside the supported range ({ApiFootballSeasonScope.FirstSeason}-{ApiFootballSeasonScope.CurrentSeason}).";
            return null;
        }

        if (!TryReadInt(args, CompetitionFlag, required: false, out var competition, ref error)
            || !TryReadInt(args, LimitFlag, required: false, out var limit, ref error))
        {
            return null;
        }

        if (limit is <= 0)
        {
            error = $"{LimitFlag} must be greater than zero.";
            return null;
        }

        if (competition is not null && ApiFootballSeasonScope.For(season.Value).All(e => e.LeagueId != competition))
        {
            error = $"Competition {competition} is not in the scope of season {season}.";
            return null;
        }

        return new SeasonImportRequest(season.Value, competition, limit);
    }

    internal static bool TryReadInt(string[] args, string flag, bool required, out int? value, ref string? error)
    {
        value = null;
        var index = Array.IndexOf(args, flag);
        if (index < 0)
        {
            if (required)
            {
                error = $"{flag} is required.";
                return false;
            }

            return true;
        }

        if (index + 1 >= args.Length || !int.TryParse(args[index + 1], NumberStyles.None, CultureInfo.InvariantCulture, out var parsed))
        {
            error = $"{flag} expects a number.";
            return false;
        }

        value = parsed;
        return true;
    }
}
