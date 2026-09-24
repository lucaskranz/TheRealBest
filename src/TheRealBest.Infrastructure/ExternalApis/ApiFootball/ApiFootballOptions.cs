namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

public sealed class ApiFootballOptions
{
    public const string SectionName = "ApiFootball";

    public string BaseUrl { get; set; } = "https://v3.football.api-sports.io";

    /// <summary>Chave da api-sports.io. Nunca versionar: usar dotnet user-secrets ou variável de ambiente.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Limite de requisições por minuto do plano (Free = 10, Pro = 300). Exceder o limite pode levar ao bloqueio da conta,
    /// então o cliente espaça as requisições para nunca passar deste valor.
    /// </summary>
    public int RequestsPerMinute { get; set; } = 10;
}
