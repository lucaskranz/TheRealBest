namespace TheRealBest.Application.Players;

using System.Text.Json;
using TheRealBest.Application.DTOs.Players;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.ValueObjects;

/// <summary>
/// Atributos do radar: pontos do recibo por 90 minutos em cada dimensão de jogo, convertidos em percentil
/// entre jogadores da mesma função tática (posições que compartilham a matriz de pesos do motor: volante com
/// meio-campista, meia-atacante com ponta). Tudo sai dos recibos gravados, então o perfil é auditável.
/// </summary>
public static class PlayerAttributeCalculator
{
    /// <summary>Minutos mínimos na temporada para um jogador servir de referência de comparação.</summary>
    public const int MinMinutesForComparison = 270;

    /// <summary>Abaixo disso, o percentil não é mostrado (amostra de comparação pequena demais).</summary>
    public const int MinPeersForPercentile = 5;

    private static readonly ActionCategory[] OutfieldAxes =
        [ActionCategory.Finishing, ActionCategory.Creation, ActionCategory.Possession, ActionCategory.Defending, ActionCategory.Duels, ActionCategory.Discipline];

    private static readonly ActionCategory[] GoalkeeperAxes =
        [ActionCategory.Goalkeeping, ActionCategory.Defending, ActionCategory.Possession, ActionCategory.Creation, ActionCategory.Duels, ActionCategory.Discipline];

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>Recibo de uma partida reduzido ao que o radar precisa.</summary>
    public sealed record ScoreSample(Guid PlayerId, PlayerPosition PrimaryPosition, int MinutesPlayed, string ActionBreakdownJson, string PenaltyBreakdownJson);

    public static IReadOnlyList<PlayerAttributeDto> Calculate(Guid playerId, PlayerPosition position, IEnumerable<MatchPerformanceScore> seasonScores) =>
        Calculate(playerId, position, seasonScores.Select(s => new ScoreSample(
            s.PlayerId, s.Player.PrimaryPosition, s.MatchPlayerStats.MinutesPlayed, s.ActionBreakdownJson, s.PenaltyBreakdownJson)));

    public static IReadOnlyList<PlayerAttributeDto> Calculate(Guid playerId, PlayerPosition position, IEnumerable<ScoreSample> seasonScores)
    {
        var profiles = seasonScores
            .GroupBy(s => s.PlayerId)
            .Select(g => new Profile(
                g.Key,
                g.First().PrimaryPosition,
                g.Sum(s => s.MinutesPlayed),
                PointsByCategory(g)))
            .ToList();

        var target = profiles.FirstOrDefault(p => p.PlayerId == playerId);
        if (target is null || target.Minutes == 0)
        {
            return [];
        }

        var peers = profiles
            .Where(p => RoleOf(p.Position) == RoleOf(position) && p.Minutes >= MinMinutesForComparison)
            .ToList();

        var axes = position == PlayerPosition.GK ? GoalkeeperAxes : OutfieldAxes;
        return axes.Select(category =>
        {
            var value = target.Per90(category);
            int? percentile = peers.Count >= MinPeersForPercentile
                ? (int)Math.Round(100m * peers.Count(p => p.Per90(category) <= value) / peers.Count, MidpointRounding.AwayFromZero)
                : null;

            return new PlayerAttributeDto(category.ToString(), Math.Round(value, 2, MidpointRounding.AwayFromZero), percentile, peers.Count);
        }).ToList();
    }

    /// <summary>Espelha o agrupamento de matrizes em ScoringRulesProvider (CDM/CM e CAM/W usam os mesmos pesos).</summary>
    public static PlayerPosition RoleOf(PlayerPosition position) => position switch
    {
        PlayerPosition.CM => PlayerPosition.CDM,
        PlayerPosition.W => PlayerPosition.CAM,
        _ => position,
    };

    private static Dictionary<ActionCategory, decimal> PointsByCategory(IEnumerable<ScoreSample> scores) =>
        scores
            .SelectMany(s => Items(s.ActionBreakdownJson).Concat(Items(s.PenaltyBreakdownJson)))
            .GroupBy(i => i.Category)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalPoints));

    private static IEnumerable<ActionScoreItem> Items(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<ActionScoreItem>>(json, JsonOptions) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private sealed record Profile(Guid PlayerId, PlayerPosition Position, int Minutes, Dictionary<ActionCategory, decimal> Points)
    {
        public decimal Per90(ActionCategory category) =>
            Minutes == 0 ? 0m : Points.GetValueOrDefault(category) / Minutes * 90m;
    }
}
