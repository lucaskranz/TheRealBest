namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

/// <summary>
/// Garante que as requisições saiam em fila, espaçadas para nunca exceder o limite por minuto do plano,
/// e interrompe a ingestão quando a cota diária acaba (em vez de insistir e arriscar bloqueio da conta).
/// Singleton: o estado precisa ser compartilhado por todas as instâncias do handler HTTP.
/// </summary>
public sealed class ApiFootballRequestPacer(IOptions<ApiFootballOptions> options, TimeProvider timeProvider)
{
    public const string MinuteRemainingHeader = "x-ratelimit-remaining";
    public const string DailyRemainingHeader = "x-ratelimit-requests-remaining";

    private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private DateTimeOffset _nextAllowedAt = DateTimeOffset.MinValue;

    public int? DailyRemaining { get; private set; }

    public TimeSpan MinInterval =>
        TimeSpan.FromMilliseconds(Math.Ceiling(OneMinute.TotalMilliseconds / Math.Max(1, options.Value.RequestsPerMinute)));

    /// <summary>Aguarda a vez da próxima requisição.</summary>
    public async Task WaitTurnAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (DailyRemaining <= 0)
            {
                throw new ApiFootballException(
                    ApiFootballErrorKind.DailyQuotaExceeded,
                    "API-Football daily quota exhausted; it resets at 00:00 UTC.");
            }

            var now = timeProvider.GetUtcNow();
            var wait = _nextAllowedAt - now;
            if (wait > TimeSpan.Zero)
            {
                await Task.Delay(wait, timeProvider, cancellationToken);
                now = timeProvider.GetUtcNow();
            }

            _nextAllowedAt = now + MinInterval;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Atualiza o estado a partir dos cabeçalhos de limite devolvidos pela API.</summary>
    public void Observe(HttpResponseHeaders headers)
    {
        if (TryReadInt(headers, DailyRemainingHeader, out var daily))
        {
            DailyRemaining = daily;
        }

        // Janela do minuto esgotada: espera um minuto inteiro antes da próxima
        if (TryReadInt(headers, MinuteRemainingHeader, out var minute) && minute <= 0)
        {
            var resumeAt = timeProvider.GetUtcNow() + OneMinute;
            if (resumeAt > _nextAllowedAt)
            {
                _nextAllowedAt = resumeAt;
            }
        }
    }

    private static bool TryReadInt(HttpResponseHeaders headers, string name, out int value)
    {
        value = 0;
        return headers.TryGetValues(name, out var values) && int.TryParse(values.FirstOrDefault(), out value);
    }
}
