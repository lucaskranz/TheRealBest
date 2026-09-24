namespace TheRealBest.Application.UseCases;

using TheRealBest.Application.DTOs.Common;
using TheRealBest.Application.DTOs.Matches;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Interfaces;

public sealed class GetPagedMatchesUseCase(IMatchRepository matchRepository) : IGetPagedMatchesUseCase
{
    public async Task<PagedResult<MatchSummaryDto>> ExecuteAsync(MatchFilterDto filter, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await matchRepository.GetPagedMatchesAsync(
            filter.SeasonYear,
            filter.CompetitionId,
            filter.TeamId,
            filter.Page,
            filter.PageSize,
            cancellationToken
        );

        var dtoList = items.Select(m => new MatchSummaryDto(
            Id: m.Id,
            MatchDate: m.MatchDate,
            CompetitionName: m.Competition?.Name ?? string.Empty,
            CompetitionTier: m.Competition?.Tier.ToString() ?? string.Empty,
            RoundPhase: m.RoundPhase,
            IsKnockout: m.IsKnockout,
            HomeTeam: new TeamSummaryDto(m.HomeTeam.Id, m.HomeTeam.Name, m.HomeTeam.ShortName, m.HomeTeam.LogoUrl, m.HomeTeam.Country, m.HomeTeam.EloRanking),
            AwayTeam: new TeamSummaryDto(m.AwayTeam.Id, m.AwayTeam.Name, m.AwayTeam.ShortName, m.AwayTeam.LogoUrl, m.AwayTeam.Country, m.AwayTeam.EloRanking),
            HomeScore: m.HomeScore,
            AwayScore: m.AwayScore,
            EvaluatedPlayersCount: m.PerformanceScores.Count
        )).ToList();

        return new PagedResult<MatchSummaryDto>(dtoList, totalCount, filter.Page, filter.PageSize);
    }
}