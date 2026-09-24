namespace TheRealBest.Application.UseCases;

using System.Text.Json;
using TheRealBest.Application.DTOs.Common;
using TheRealBest.Application.DTOs.Ranking;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.Specifications;

public sealed class GetSeasonRankingUseCase(
    IRankingRepository rankingRepository,
    IMatchRepository matchRepository,
    ActionLabelResolver labels) : IGetSeasonRankingUseCase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<PagedResult<SeasonRankingItemDto>> ExecuteAsync(RankingFilterDto filter, CancellationToken cancellationToken = default)
    {
        var spec = new PlayerRankingSpec(
            SeasonYear: filter.SeasonYear,
            Position: filter.Position,
            CompetitionId: null,
            Nationality: filter.Nationality,
            Page: Math.Max(1, filter.Page),
            PageSize: Math.Clamp(filter.PageSize, 1, 100),
            Search: filter.Search
        );

        var rankings = await rankingRepository.GetRankingsAsync(spec, cancellationToken);
        var totalCount = await rankingRepository.CountRankingsAsync(spec, cancellationToken);

        var teams = await matchRepository.GetLatestClubTeamsAsync(rankings.Select(r => r.PlayerId).ToList(), spec.SeasonYear, cancellationToken);
        var items = rankings.Select(r => MapToDto(r, teams.GetValueOrDefault(r.PlayerId))).ToList();

        return new PagedResult<SeasonRankingItemDto>(items, totalCount, spec.Page, spec.PageSize);
    }

    public async Task<IReadOnlyList<SeasonRankingItemDto>> GetTopContendersAsync(int seasonYear = 2023, int count = 5, CancellationToken cancellationToken = default)
    {
        var spec = new PlayerRankingSpec(
            SeasonYear: seasonYear,
            Position: null,
            CompetitionId: null,
            Nationality: null,
            Page: 1,
            PageSize: Math.Clamp(count, 1, 50)
        );

        var rankings = await rankingRepository.GetRankingsAsync(spec, cancellationToken);
        var teams = await matchRepository.GetLatestClubTeamsAsync(rankings.Select(r => r.PlayerId).ToList(), seasonYear, cancellationToken);
        return rankings.Select(r => MapToDto(r, teams.GetValueOrDefault(r.PlayerId))).ToList();
    }

    private SeasonRankingItemDto MapToDto(SeasonRanking ranking, Team? team)
    {
        var topMatches = DeserializeTopMatches(ranking.Top5MatchesJson);

        return new SeasonRankingItemDto(
            PlayerId: ranking.PlayerId,
            PlayerName: ranking.Player?.Name ?? string.Empty,
            Nationality: ranking.Player?.Nationality ?? string.Empty,
            PhotoUrl: ranking.Player?.PhotoUrl,
            PrimaryPosition: ranking.Player?.PrimaryPosition.ToString() ?? string.Empty,
            PrimaryPositionLabel: ranking.Player is null ? string.Empty : labels.PositionLabel(ranking.Player.PrimaryPosition.ToString(), SupportedLocales.Current),
            TeamName: team?.Name,
            TeamLogoUrl: team?.LogoUrl,
            OverallRank: ranking.OverallRank,
            PositionRank: ranking.PositionRank,
            FssScore: ranking.FssScore,
            MpsAverage: ranking.MpsAverage,
            PresenceFactor: ranking.PresenceFactor,
            ClutchIndex: ranking.ClutchIndex,
            TotalMatches: ranking.TotalMatches,
            TotalMinutes: ranking.TotalMinutes,
            IsRankingEligible: ranking.IsRankingEligible,
            RecalculatedAt: ranking.RecalculatedAt,
            TopMatches: topMatches
        );
    }

    private static IReadOnlyList<Top5MatchItemDto> DeserializeTopMatches(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];

        try
        {
            return JsonSerializer.Deserialize<List<Top5MatchItemDto>>(json, JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }
}