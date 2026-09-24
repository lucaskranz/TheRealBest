namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Text.Json;

public static class ApiFootballJson
{
    /// <summary>
    /// Nomes em minúsculas/camelCase, sem diferenciar maiúsculas, e números que às vezes chegam como string.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web);
}
