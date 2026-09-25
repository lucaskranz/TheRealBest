namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using TheRealBest.Domain.Enums;

/// <summary>
/// Infere a posição tática de cada jogador na partida. A API informa só G/D/M/F; a escalação traz a formação
/// e o grid "linha:coluna" dos titulares, de onde saem as posições do motor de pontuação.
/// </summary>
/// <remarks>
/// Regras para titulares (linha 1 = goleiro, 2 = defesa, última = ataque, as do meio = meio-campo):
/// defesa com 4+ jogadores tem laterais nas pontas, senão só zagueiros; alas da primeira linha de meio (4+) à frente de
/// 3 zagueiros são laterais; extremidades de uma linha de meio com 4+ jogadores, ou da linha logo atrás do ataque, são
/// pontas (W); com duas linhas de meio, a primeira é de volantes e a segunda de meias-atacantes; ataque com 3+ tem
/// pontas nas extremidades.
/// Reservas herdam a posição de quem substituíram quando compatível com a letra da API; senão, a posição padrão da letra.
/// </remarks>
public static class PositionResolver
{
    private static readonly IReadOnlyDictionary<string, PlayerPosition[]> CompatibleByLetter = new Dictionary<string, PlayerPosition[]>
    {
        ["G"] = [PlayerPosition.GK],
        ["D"] = [PlayerPosition.CB, PlayerPosition.FB],
        ["M"] = [PlayerPosition.CDM, PlayerPosition.CM, PlayerPosition.CAM, PlayerPosition.W, PlayerPosition.FB],
        ["F"] = [PlayerPosition.ST, PlayerPosition.W, PlayerPosition.CAM],
    };

    public static IReadOnlyDictionary<int, PlayerPosition> Resolve(
        IEnumerable<LineupItem> lineups,
        IEnumerable<MatchTimeline.Substitution> substitutions,
        IReadOnlyDictionary<int, string?> letterByPlayer)
    {
        var positions = new Dictionary<int, PlayerPosition>();

        foreach (var lineup in lineups)
        {
            foreach (var (playerId, position) in ResolveStarters(lineup))
            {
                positions[playerId] = position;
            }
        }

        foreach (var sub in substitutions)
        {
            var letter = letterByPlayer.GetValueOrDefault(sub.PlayerInId);
            positions[sub.PlayerInId] =
                positions.TryGetValue(sub.PlayerOutId, out var replaced) && IsCompatible(letter, replaced)
                    ? replaced
                    : DefaultFor(letter);
        }

        foreach (var (playerId, letter) in letterByPlayer)
        {
            positions.TryAdd(playerId, DefaultFor(letter));
        }

        return positions;
    }

    public static PlayerPosition DefaultFor(string? letter) => letter?.ToUpperInvariant() switch
    {
        "G" => PlayerPosition.GK,
        "D" => PlayerPosition.CB,
        "F" => PlayerPosition.ST,
        _ => PlayerPosition.CM,
    };

    private static bool IsCompatible(string? letter, PlayerPosition position) =>
        letter is not null
        && CompatibleByLetter.TryGetValue(letter.ToUpperInvariant(), out var compatible)
        && compatible.Contains(position);

    private static IEnumerable<(int PlayerId, PlayerPosition Position)> ResolveStarters(LineupItem lineup)
    {
        var slots = (lineup.StartXI ?? [])
            .Select(s => (s.Player, Grid: ParseGrid(s.Player.Grid)))
            .ToList();

        var outfieldRows = slots
            .Where(s => s.Grid is { Row: > 1 })
            .Select(s => s.Grid!.Value.Row)
            .Distinct()
            .Order()
            .ToList();

        var rowSizes = slots.Where(s => s.Grid is not null).GroupBy(s => s.Grid!.Value.Row).ToDictionary(g => g.Key, g => g.Count());

        foreach (var (player, grid) in slots)
        {
            if (grid is null || outfieldRows.Count == 0)
            {
                yield return (player.Id, DefaultFor(player.Pos));
                continue;
            }

            yield return (player.Id, FromGrid(player.Pos, grid.Value, outfieldRows, rowSizes));
        }
    }

    private static PlayerPosition FromGrid(
        string? letter,
        (int Row, int Col) grid,
        List<int> outfieldRows,
        Dictionary<int, int> rowSizes)
    {
        if (grid.Row == 1 || string.Equals(letter, "G", StringComparison.OrdinalIgnoreCase))
        {
            return PlayerPosition.GK;
        }

        var defenseRow = outfieldRows[0];
        var attackRow = outfieldRows[^1];
        var middleRows = outfieldRows.Skip(1).SkipLast(1).ToList();

        var rowSize = rowSizes[grid.Row];
        var isEdge = rowSize >= 3 && (grid.Col == 1 || grid.Col == rowSize);

        if (grid.Row == defenseRow)
        {
            return rowSize >= 4 && isEdge ? PlayerPosition.FB : PlayerPosition.CB;
        }

        if (grid.Row == attackRow && middleRows.Count > 0)
        {
            return isEdge ? PlayerPosition.W : PlayerPosition.ST;
        }

        // Alas da primeira linha de meio à frente de uma defesa de 3 (3-5-2, 3-4-3)
        var isWingBackRow = grid.Row == middleRows.FirstOrDefault() && rowSizes[defenseRow] == 3 && rowSize >= 4;
        if (isWingBackRow && isEdge)
        {
            return PlayerPosition.FB;
        }

        // Pontas de uma linha de 4+ no meio, ou da linha de meias logo atrás do ataque (os abertos do 4-2-3-1)
        var isLineBehindAttack = middleRows.Count >= 2 && grid.Row == middleRows[^1];
        if (isEdge && (rowSize >= 4 || isLineBehindAttack))
        {
            return PlayerPosition.W;
        }

        if (string.Equals(letter, "F", StringComparison.OrdinalIgnoreCase))
        {
            return PlayerPosition.CAM;
        }

        if (middleRows.Count >= 2)
        {
            if (grid.Row == middleRows[0])
            {
                return PlayerPosition.CDM;
            }

            if (grid.Row == middleRows[^1])
            {
                return PlayerPosition.CAM;
            }
        }

        return PlayerPosition.CM;
    }

    private static (int Row, int Col)? ParseGrid(string? grid)
    {
        var parts = grid?.Split(':');
        return parts is { Length: 2 } && int.TryParse(parts[0], out var row) && int.TryParse(parts[1], out var col)
            ? (row, col)
            : null;
    }
}
