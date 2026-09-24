namespace TheRealBest.Application.Localization;

using System.Globalization;

/// <summary>
/// Locales do produto. A cultura da requisição é definida pelo LocalizationMiddleware a partir do Accept-Language.
/// </summary>
public static class SupportedLocales
{
    public const string Default = "pt-BR";

    public static IReadOnlyList<string> All { get; } = ["pt-BR", "en", "es"];

    /// <summary>Locale da requisição atual (cultura de UI), ou o padrão se não for suportado.</summary>
    public static string Current => Resolve(CultureInfo.CurrentUICulture.Name);

    /// <summary>
    /// Casa uma tag de idioma (ex.: "pt", "pt-PT", "en-US", "es-419") com um locale suportado pelo idioma principal.
    /// </summary>
    public static string Resolve(string? languageTag)
    {
        if (string.IsNullOrWhiteSpace(languageTag))
        {
            return Default;
        }

        var exact = All.FirstOrDefault(l => string.Equals(l, languageTag, StringComparison.OrdinalIgnoreCase));
        if (exact is not null)
        {
            return exact;
        }

        var language = languageTag.Split('-')[0];
        return All.FirstOrDefault(l => l.Split('-')[0].Equals(language, StringComparison.OrdinalIgnoreCase)) ?? Default;
    }

    public static bool IsSupported(string? languageTag) =>
        !string.IsNullOrWhiteSpace(languageTag)
        && All.Any(l => l.Split('-')[0].Equals(languageTag.Split('-')[0], StringComparison.OrdinalIgnoreCase));
}
