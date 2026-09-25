namespace TheRealBest.Domain.Enums;

using System.Text.Json.Serialization;

/// <summary>
/// Dimensão de jogo de uma ação pontuável. Agrupa o recibo e alimenta o radar de atributos do jogador.
/// Gravada como texto no JSONB do recibo.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActionCategory
{
    Unknown = 0,
    Finishing = 1,
    Creation = 2,
    Possession = 3,
    Defending = 4,
    Duels = 5,
    Goalkeeping = 6,
    Discipline = 7,
}
