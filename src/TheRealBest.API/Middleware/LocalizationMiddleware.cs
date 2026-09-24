namespace TheRealBest.API.Middleware;

using System.Globalization;
using Microsoft.Net.Http.Headers;
using TheRealBest.Application.Localization;

/// <summary>
/// Define a cultura da requisição a partir do Accept-Language (respeitando os pesos q), casando pelo idioma principal:
/// "pt-PT" → pt-BR, "en-US" → en, "es-419" → es. Sem idioma suportado, usa pt-BR.
/// Responde com Content-Language e Vary: Accept-Language, para caches não misturarem idiomas.
/// </summary>
public sealed class LocalizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var locale = ResolveLocale(context.Request.Headers.AcceptLanguage.ToString());
        var culture = CultureInfo.GetCultureInfo(locale);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.ContentLanguage = locale;
            context.Response.Headers.Append(HeaderNames.Vary, HeaderNames.AcceptLanguage);
            return Task.CompletedTask;
        });

        await next(context);
    }

    public static string ResolveLocale(string? acceptLanguage)
    {
        if (string.IsNullOrWhiteSpace(acceptLanguage)
            || !StringWithQualityHeaderValue.TryParseList(acceptLanguage.Split(','), out var languages))
        {
            return SupportedLocales.Default;
        }

        var preferred = languages
            .Where(l => l.Quality is null or > 0)
            .OrderByDescending(l => l.Quality ?? 1)
            .Select(l => l.Value.Value)
            .FirstOrDefault(SupportedLocales.IsSupported);

        return SupportedLocales.Resolve(preferred);
    }
}
