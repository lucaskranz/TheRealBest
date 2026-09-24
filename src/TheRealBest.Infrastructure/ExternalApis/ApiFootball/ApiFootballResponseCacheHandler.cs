namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

/// <summary>
/// Cache em disco das respostas da API-Football que não mudam mais: detalhes de partidas (jogadores, eventos,
/// escalações) e listagens em que todas as partidas já estão encerradas. Respostas com erro nunca são guardadas.
/// Fica antes do pacer: um acerto no cache não consome cota nem espera a vez.
/// </summary>
public sealed class ApiFootballResponseCacheHandler(IOptions<ApiFootballOptions> options) : DelegatingHandler
{
    private static readonly HashSet<string> ClosedStatuses = ["FT", "AET", "PEN", "CANC", "ABD", "AWD", "WO"];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (!settings.ResponseCacheEnabled || request.Method != HttpMethod.Get || request.RequestUri is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var path = Path.Combine(settings.ResolvedResponseCacheDirectory, CacheFileName(request.RequestUri));
        if (File.Exists(path))
        {
            return JsonResponse(await File.ReadAllTextAsync(path, cancellationToken), request);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (IsCacheable(request.RequestUri, body))
        {
            Directory.CreateDirectory(settings.ResolvedResponseCacheDirectory);
            await File.WriteAllTextAsync(path, body, cancellationToken);
        }

        // O conteúdo original já foi lido: devolve uma cópia mantendo status e cabeçalhos
        response.Content = new StringContent(body, Encoding.UTF8, "application/json");
        return response;
    }

    /// <summary>Ex.: /fixtures/players?fixture=1184780 → fixtures_players_fixture-1184780.json</summary>
    public static string CacheFileName(Uri uri)
    {
        var path = uri.AbsolutePath.Trim('/').Replace('/', '_');
        var query = uri.Query.TrimStart('?').Replace('&', '_').Replace('=', '-');
        var name = string.IsNullOrEmpty(query) ? path : $"{path}_{query}";
        return string.Concat(name.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c)) + ".json";
    }

    private static bool IsCacheable(Uri uri, string body)
    {
        try
        {
            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;

            var errors = root.GetProperty("errors");
            var hasErrors = errors.ValueKind == JsonValueKind.Object
                ? errors.EnumerateObject().Any()
                : errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0;
            if (hasErrors || root.GetProperty("results").GetInt32() == 0)
            {
                return false;
            }

            // Listagem de partidas: só é imutável se nenhuma partida estiver por jogar ou em andamento
            if (uri.AbsolutePath.TrimEnd('/').Equals("/fixtures", StringComparison.OrdinalIgnoreCase))
            {
                return root.GetProperty("response").EnumerateArray().All(item =>
                    ClosedStatuses.Contains(item.GetProperty("fixture").GetProperty("status").GetProperty("short").GetString() ?? string.Empty));
            }

            return true;
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            return false;
        }
    }

    private static HttpResponseMessage JsonResponse(string body, HttpRequestMessage request) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
            RequestMessage = request,
        };
}
