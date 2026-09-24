namespace TheRealBest.Application.UseCases;

using System.Text.Json;
using TheRealBest.Application.DTOs.Players;
using TheRealBest.Application.DTOs.Ranking;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;

public sealed class GetPlayerProfileUseCase(
    IPlayerRepository playerRepository,
    IRankingRepository rankingRepository,
    IMatchRepository matchRepository
) : IGetPlayerProfileUseCase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<PlayerProfileDto?> ExecuteAsync(Guid playerId, int seasonYear = 2023, CancellationToken cancellationToken = default)
    {
        var player = await playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player is null) return null;

        var ranking = await rankingRepository.GetByPlayerAndSeasonAsync(playerId, seasonYear, cancellationToken);
        var matchStats = await matchRepository.GetPlayerStatsByPlayerAndSeasonAsync(playerId, seasonYear, cancellationToken);

        var rankingDto = ranking is not null ? MapRankingDto(ranking, player) : null;

        var recentMatches = matchStats.Select(s => new PlayerMatchStatDto(
            MatchId: s.MatchId,
            MatchDate: s.Match.MatchDate,
            CompetitionName: s.Match.Competition.Name,
            HomeTeamName: s.Match.HomeTeam.Name,
            AwayTeamName: s.Match.AwayTeam.Name,
            HomeScore: s.Match.HomeScore,
            AwayScore: s.Match.AwayScore,
            MinutesPlayed: s.MinutesPlayed,
            Goals: s.Goals,
            Assists: s.Assists,
            YellowCards: s.YellowCards,
            RedCards: s.RedCards,
            FinalMps: s.PerformanceScore?.FinalMps
        )).ToList();

        return new PlayerProfileDto(
            Id: player.Id,
            Name: player.Name,
            Nationality: player.Nationality,
            PrimaryPosition: player.PrimaryPosition.ToString(),
            PhotoUrl: player.PhotoUrl,
            DateOfBirth: player.DateOfBirth,
            SeasonRanking: rankingDto,
            RecentMatches: recentMatches
        );
    }

    private static SeasonRankingItemDto MapRankingDto(SeasonRanking ranking, Player player)
    {
        IReadOnlyList<Top5MatchItemDto> topMatches = [];
        if (!string.IsNullOrWhiteSpace(ranking.Top5MatchesJson))
        {
            try
            {
                topMatches = JsonSerializer.Deserialize<List<Top5MatchItemDto>>(ranking.Top5MatchesJson, JsonOptions) ?? [];
            }
            catch { }
        }

        return new SeasonRankingItemDto(
            PlayerId: ranking.PlayerId,
            PlayerName: player.Name,
            Nationality: player.Nationality,
            PhotoUrl: player.PhotoUrl,
            PrimaryPosition: player.PrimaryPosition.ToString(),
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
}