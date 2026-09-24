namespace TheRealBest.Domain.Specifications;

using TheRealBest.Domain.Enums;

/// <summary>
/// EspecificaÃ§Ã£o de filtros e paginaÃ§Ã£o para consulta do ranking de jogadores.
/// </summary>
public sealed record PlayerRankingSpec(
    int SeasonYear,
    PlayerPosition? Position = null,
    Guid? CompetitionId = null,
    string? Nationality = null,
    int Page = 1,
    int PageSize = 25
)
{
    public int Skip => Math.Max(0, (Page - 1) * PageSize);
    public int Take => Math.Clamp(PageSize, 1, 100);
}