namespace TheRealBest.Application.DTOs.Players;

/// <param name="Category">Dimensão de jogo (ActionCategory: Finishing, Creation, Possession, Defending, Duels, Goalkeeping, Discipline).</param>
/// <param name="Per90">Pontos do recibo por 90 minutos nessa dimensão (negativos em Disciplina).</param>
/// <param name="Percentile">Percentil (0–100) entre jogadores da mesma posição; nulo se houver poucos para comparar.</param>
/// <param name="PeerCount">Quantos jogadores da mesma posição entraram na comparação.</param>
public sealed record PlayerAttributeDto(string Category, decimal Per90, int? Percentile, int PeerCount);
