namespace TheRealBest.Infrastructure.ExternalApis.ClubElo;

using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using TheRealBest.Domain.Interfaces;

/// <summary>
/// Ratings do ClubElo (api.clubelo.com, gratuito, sem chave). Uma requisição por data devolve todos os clubes em CSV
/// ("Rank,Club,Country,Level,Elo,From,To"); o resultado fica em memória e, se configurado, em disco.
/// Falhas da fonte ou clubes não encontrados resultam em null (multiplicador neutro), com aviso no log.
/// </summary>
public sealed class ClubEloProvider(HttpClient httpClient, ClubEloOptions options, ILogger<ClubEloProvider> logger) : IClubEloProvider
{
    public const string HttpClientName = "ClubElo";

    private readonly ConcurrentDictionary<DateOnly, Task<IReadOnlyDictionary<string, int>?>> _ratingsByDate = new();
    private readonly ConcurrentDictionary<string, byte> _warnedTeams = new();

    /// <summary>Falhas seguidas da fonte a partir das quais ela é dada como fora do ar até o fim da execução.</summary>
    public const int MaxConsecutiveFailures = 3;

    private int _consecutiveFailures;

    public async Task<int?> GetEloAsync(string teamName, DateOnly date, CancellationToken cancellationToken = default)
    {
        var ratings = await _ratingsByDate.GetOrAdd(date, d => LoadAsync(d, cancellationToken));
        if (ratings is null)
        {
            return null;
        }

        if (ratings.TryGetValue(ClubEloNames.Normalize(ClubEloNames.ToClubEloName(teamName)), out var elo))
        {
            return elo;
        }

        if (_warnedTeams.TryAdd(teamName, 0))
        {
            logger.LogWarning("ClubElo: clube '{Team}' não encontrado; adversário será tratado como neutro. Adicione um apelido em ClubEloNames.", teamName);
        }

        return null;
    }

    private async Task<IReadOnlyDictionary<string, int>?> LoadAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var fileName = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var cachePath = options.CacheDirectory is { Length: > 0 } dir ? Path.Combine(dir, fileName + ".csv") : null;

        string csv;
        if (cachePath is not null && File.Exists(cachePath))
        {
            csv = await File.ReadAllTextAsync(cachePath, cancellationToken);
        }
        else
        {
            // Fonte fora do ar: não espera o timeout de cada data nova (o Elo pode ser preenchido depois com --backfill-elo)
            if (Volatile.Read(ref _consecutiveFailures) >= MaxConsecutiveFailures)
            {
                return null;
            }

            try
            {
                csv = await httpClient.GetStringAsync(fileName, cancellationToken);
                Interlocked.Exchange(ref _consecutiveFailures, 0);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("ClubElo indisponível para {Date} ({Error}); adversários serão tratados como neutros.", fileName, ex.Message);
                if (Interlocked.Increment(ref _consecutiveFailures) == MaxConsecutiveFailures)
                {
                    logger.LogWarning("ClubElo: {Count} falhas seguidas, fonte considerada fora do ar até o fim desta execução. Use --backfill-elo quando ela voltar.",
                        MaxConsecutiveFailures);
                }

                return null;
            }

            if (cachePath is not null && ParseCsv(csv).Count > 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
                await File.WriteAllTextAsync(cachePath, csv, cancellationToken);
            }
        }

        var ratings = ParseCsv(csv);
        if (ratings.Count == 0)
        {
            logger.LogWarning("ClubElo devolveu um CSV vazio ou em formato inesperado para {Date}.", fileName);
            return null;
        }

        return ratings;
    }

    /// <summary>Lê o CSV localizando as colunas "Club" e "Elo" pelo cabeçalho.</summary>
    public static IReadOnlyDictionary<string, int> ParseCsv(string csv)
    {
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length < 2)
        {
            return new Dictionary<string, int>();
        }

        var header = lines[0].Split(',');
        var clubColumn = Array.FindIndex(header, h => h.Equals("Club", StringComparison.OrdinalIgnoreCase));
        var eloColumn = Array.FindIndex(header, h => h.Equals("Elo", StringComparison.OrdinalIgnoreCase));
        if (clubColumn < 0 || eloColumn < 0)
        {
            return new Dictionary<string, int>();
        }

        var ratings = new Dictionary<string, int>();
        foreach (var line in lines.Skip(1))
        {
            var fields = line.Split(',');
            if (fields.Length <= Math.Max(clubColumn, eloColumn)
                || !double.TryParse(fields[eloColumn], NumberStyles.Float, CultureInfo.InvariantCulture, out var elo))
            {
                continue;
            }

            ratings.TryAdd(ClubEloNames.Normalize(fields[clubColumn]), (int)Math.Round(elo, MidpointRounding.AwayFromZero));
        }

        return ratings;
    }
}

public sealed class ClubEloOptions
{
    public const string SectionName = "ClubElo";

    public string BaseUrl { get; set; } = "http://api.clubelo.com/";

    /// <summary>Pasta de cache dos CSVs por data (ratings passados não mudam). Vazio = sem cache em disco.</summary>
    public string CacheDirectory { get; set; } = string.Empty;
}

/// <summary>
/// Correspondência entre nomes da API-Football e do ClubElo. A comparação ignora acentos, maiúsculas,
/// espaços, pontuação e sufixos comuns; os apelidos cobrem nomes realmente diferentes.
/// Os apelidos ainda não foram conferidos contra a API ao vivo (fora do ar na implementação): nomes sem
/// correspondência aparecem no log e devem ser adicionados aqui.
/// </summary>
public static class ClubEloNames
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Manchester City"] = "Man City",
        ["Manchester United"] = "Man United",
        ["Bayern Munich"] = "Bayern",
        ["Bayern München"] = "Bayern",
        ["Borussia Dortmund"] = "Dortmund",
        ["Bayer Leverkusen"] = "Leverkusen",
        ["Borussia Monchengladbach"] = "Gladbach",
        ["Werder Bremen"] = "Werder",
        ["Atletico Madrid"] = "Atletico",
        ["FC Copenhagen"] = "FC Kobenhavn",
        ["BSC Young Boys"] = "Young Boys",
        ["Paris Saint Germain"] = "Paris SG",
        ["AC Milan"] = "Milan",
    };

    private static readonly string[] Affixes = ["fc", "cf", "afc", "sc", "ac", "ssc", "1"];

    public static string ToClubEloName(string apiFootballName) =>
        Aliases.TryGetValue(apiFootballName.Trim(), out var alias) ? alias : apiFootballName;

    public static string Normalize(string name)
    {
        var decomposed = name.Trim().Normalize(NormalizationForm.FormD);
        var withoutAccents = new string(decomposed.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

        var words = withoutAccents.ToLowerInvariant()
            .Split([' ', '-', '.', '\''], StringSplitOptions.RemoveEmptyEntries)
            .Where(w => !Affixes.Contains(w));

        return string.Concat(words);
    }
}
