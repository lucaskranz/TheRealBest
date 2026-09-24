namespace TheRealBest.Domain.Specifications;

using TheRealBest.Domain.Enums;

/// <summary>
/// Especificação de filtros e paginação para consulta do ranking de jogadores.
/// </summary>
public sealed record PlayerRankingSpec(
    int SeasonYear,
    PlayerPosition? Position = null,
    Guid? CompetitionId = null,
    string? Nationality = null,
    int Page = 1,
    int PageSize = 25,
    string? Search = null
)
{
    public int Skip => Math.Max(0, (Page - 1) * PageSize);
    public int Take => Math.Clamp(PageSize, 1, 100);
}