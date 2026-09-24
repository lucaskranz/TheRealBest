namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Text.Json;

public enum ApiFootballErrorKind
{
    /// <summary>Chave ausente ou inválida.</summary>
    InvalidKey,

    /// <summary>A temporada não está disponível no plano atual (o Free só acessa temporadas antigas).</summary>
    SeasonNotAvailable,

    /// <summary>Cota diária esgotada. Zera às 00:00 UTC.</summary>
    DailyQuotaExceeded,

    /// <summary>Limite por minuto excedido.</summary>
    RateLimited,

    Other,
}

public sealed class ApiFootballException(ApiFootballErrorKind kind, string message) : Exception(message)
{
    public ApiFootballErrorKind Kind { get; } = kind;

    /// <summary>
    /// Lança se o envelope trouxer erros. A API responde HTTP 200 mesmo para erros de plano, cota ou chave.
    /// </summary>
    public static void ThrowIfErrors<T>(ApiFootballResponse<T> response, string endpoint)
    {
        var errors = response.Errors;
        if (errors.ValueKind == JsonValueKind.Object)
        {
            foreach (var error in errors.EnumerateObject())
            {
                throw new ApiFootballException(KindFor(error.Name), $"API-Football {endpoint}: {error.Value}");
            }
        }
        else if (errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
        {
            throw new ApiFootballException(ApiFootballErrorKind.Other, $"API-Football {endpoint}: {errors}");
        }
    }

    private static ApiFootballErrorKind KindFor(string errorKey) => errorKey.ToLowerInvariant() switch
    {
        "token" or "access" => ApiFootballErrorKind.InvalidKey,
        "plan" => ApiFootballErrorKind.SeasonNotAvailable,
        "requests" => ApiFootballErrorKind.DailyQuotaExceeded,
        "ratelimit" => ApiFootballErrorKind.RateLimited,
        _ => ApiFootballErrorKind.Other,
    };
}
