namespace TheRealBest.Domain.ValueObjects;

using TheRealBest.Domain.Enums;

/// <summary>
/// Item detalhado do extrato auditável de pontuação da partida.
/// TotalPoints = Count × UnitWeight × MinutesFactor.
/// </summary>
/// <param name="ActionKey">Chave estável da ação em snake_case (ex.: "goal", "tackle"), usada para tradução.</param>
/// <param name="Label">Rótulo exibido; o motor grava a própria chave e a camada de localização o substitui.</param>
/// <param name="Count">Quantidade da ação. É decimal porque métricas como xA e xG superado são fracionárias.</param>
/// <param name="UnitWeight">Peso posicional de uma unidade da ação.</param>
/// <param name="TotalPoints">Pontos resultantes, já com o fator de minutos aplicado.</param>
/// <param name="Minute">Minuto do lance, quando disponível.</param>
/// <param name="MinutesFactor">Fator de minutos aplicado ao item (1.0 para ações decisivas, que não são proporcionalizadas).</param>
/// <param name="Category">Dimensão de jogo da ação (finalização, criação, defesa...).</param>
public sealed record ActionScoreItem(
    string ActionKey,
    string Label,
    decimal Count,
    decimal UnitWeight,
    decimal TotalPoints,
    int? Minute = null,
    decimal MinutesFactor = 1.0m,
    ActionCategory Category = ActionCategory.Unknown
);
