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

    /// <summary>
    /// Busca os detalhes de até 20 partidas por requisição (parâmetro "ids", só nos planos pagos) em vez de 3 requisições
    /// por partida. Ligar junto com a assinatura Pro.
    /// </summary>
    public bool BatchFixtureDetails { get; set; }

    /// <summary>
    /// Guarda em disco as respostas de partidas encerradas (dados que não mudam mais). Recriar o banco ou repetir
    /// uma importação passa a custar zero requisições — essencial no plano Free (100/dia).
    /// </summary>
    public bool ResponseCacheEnabled { get; set; }

    /// <summary>Pasta do cache. Vazio = %LOCALAPPDATA%/TheRealBest/api-football-cache (fora do repositório).</summary>
    public string ResponseCacheDirectory { get; set; } = string.Empty;

    public string ResolvedResponseCacheDirectory =>
        string.IsNullOrWhiteSpace(ResponseCacheDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TheRealBest", "api-football-cache")
            : ResponseCacheDirectory;
}
