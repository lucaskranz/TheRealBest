namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Net;
using Microsoft.Extensions.Options;

/// <summary>
/// Adiciona a chave da api-sports.io em todas as requisições.
/// </summary>
public sealed class ApiFootballAuthHandler(IOptions<ApiFootballOptions> options) : DelegatingHandler
{
    public const string ApiKeyHeader = "x-apisports-key";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var apiKey = options.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ApiFootballException(
                ApiFootballErrorKind.InvalidKey,
                $"{ApiFootballOptions.SectionName}:ApiKey is not configured. Set it with dotnet user-secrets.");
        }

        request.Headers.Remove(ApiKeyHeader);
        request.Headers.Add(ApiKeyHeader, apiKey);
        return base.SendAsync(request, cancellationToken);
    }
}

/// <summary>
/// Espaça as requisições conforme o plano e reage aos cabeçalhos de limite. Um HTTP 429 não é repetido.
/// </summary>
public sealed class ApiFootballRateLimitHandler(ApiFootballRequestPacer pacer) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await pacer.WaitTurnAsync(cancellationToken);

        var response = await base.SendAsync(request, cancellationToken);
        pacer.Observe(response.Headers);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            response.Dispose();
            throw new ApiFootballException(ApiFootballErrorKind.RateLimited, "API-Football rate limit exceeded (HTTP 429).");
        }

        return response;
    }
}
