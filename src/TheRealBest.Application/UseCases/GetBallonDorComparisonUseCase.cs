namespace TheRealBest.Application.UseCases;

using TheRealBest.Application.Comparison;
using TheRealBest.Application.DTOs.Comparison;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.Specifications;

/// <summary>
/// Cruza a classificação oficial da Bola de Ouro com o Fair Season Score da temporada avaliada.
/// Indicados sem partidas importadas aparecem como "dados insuficientes", nunca com nota estimada.
/// </summary>
public sealed class GetBallonDorComparisonUseCase(
    IRankingRepository rankingRepository,
    ActionLabelResolver labels) : IGetBallonDorComparisonUseCase
{
    /// <summary>Quantos jogadores do topo do índice são verificados na lista de "fora da lista oficial".</summary>
    public const int UnnominatedScanSize = 30;

    public async Task<BallonDorComparisonDto?> ExecuteAsync(int year, CancellationToken cancellationToken = default)
    {
        var edition = BallonDorCatalog.ForYear(year);
        if (edition is null)
        {
            return null;
        }

        var externalIds = edition.Nominees
            .Where(n => n.ExternalApiId is not null)
            .Select(n => n.ExternalApiId!)
            .ToList();

        var nomineeRankings = (await rankingRepository.GetByExternalPlayerIdsAsync(edition.SeasonYear, externalIds, cancellationToken))
            .ToDictionary(r => r.Player.ExternalApiId);

        var eligibleSpec = new PlayerRankingSpec(edition.SeasonYear, PageSize: UnnominatedScanSize);
        var eligibleCount = await rankingRepository.CountRankingsAsync(eligibleSpec, cancellationToken);
        var topOfIndex = await rankingRepository.GetRankingsAsync(eligibleSpec, cancellationToken);

        var locale = SupportedLocales.Current;
        var entries = edition.Nominees
            .Select(n => ToEntry(n, n.ExternalApiId is null ? null : nomineeRankings.GetValueOrDefault(n.ExternalApiId), locale))
            .ToList();

        var nominated = externalIds.ToHashSet();
        var unnominated = topOfIndex
            .Where(r => !nominated.Contains(r.Player.ExternalApiId))
            .Select(r => new UnnominatedPlayerDto(
                r.PlayerId,
                r.Player.Name,
                r.Player.PhotoUrl,
                labels.PositionLabel(r.Player.PrimaryPosition.ToString(), locale),
                r.OverallRank,
                r.FssScore))
            .ToList();

        return new BallonDorComparisonDto(
            edition.Year,
            edition.SeasonYear,
            edition.SourceName,
            edition.SourceUrl,
            eligibleCount,
            entries,
            unnominated);
    }

    private BallonDorEntryDto ToEntry(BallonDorNominee nominee, SeasonRanking? ranking, string locale)
    {
        if (ranking is null || ranking.TotalMatches == 0)
        {
            return new BallonDorEntryDto(
                nominee.OfficialRank, nominee.Name, nominee.Club, ComparisonStatus.NoData,
                ranking?.PlayerId, ranking?.Player.PhotoUrl, null, null, null, null, null, null);
        }

        var eligible = ranking.IsRankingEligible && ranking.OverallRank > 0;
        int? ourRank = eligible ? ranking.OverallRank : null;

        return new BallonDorEntryDto(
            nominee.OfficialRank,
            nominee.Name,
            nominee.Club,
            eligible ? ComparisonStatus.Ranked : ComparisonStatus.NotEligible,
            ranking.PlayerId,
            ranking.Player.PhotoUrl,
            labels.PositionLabel(ranking.Player.PrimaryPosition.ToString(), locale),
            ourRank,
            ourRank is null ? null : nominee.OfficialRank - ourRank,
            ranking.FssScore,
            ranking.TotalMatches,
            ranking.TotalMinutes);
    }
}
