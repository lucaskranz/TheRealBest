namespace TheRealBest.Application.UseCases;

using TheRealBest.Application.DTOs.Matches;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Interfaces;

public sealed class GetMatchDetailUseCase(IMatchRepository matchRepository, ActionLabelResolver labels) : IGetMatchDetailUseCase
{
    public async Task<MatchDetailDto?> ExecuteAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        var match = await matchRepository.GetWithStatsByIdAsync(matchId, cancellationToken);
        if (match is null) return null;

        var scoresMap = match.PerformanceScores.ToDictionary(s => s.PlayerId);

        var playerPerformances = match.PlayerStats
            .OrderBy(ps => ps.TeamId == match.HomeTeamId ? 0 : 1)
            .ThenByDescending(ps => scoresMap.TryGetValue(ps.PlayerId, out var s) ? s.FinalMps : 0)
            .Select(ps =>
            {
                scoresMap.TryGetValue(ps.PlayerId, out var score);
                return new MatchPlayerPerformanceDto(
                    PlayerId: ps.PlayerId,
                    PlayerName: ps.Player?.Name ?? string.Empty,
                    PhotoUrl: ps.Player?.PhotoUrl,
                    PositionPlayed: ps.PositionPlayed.ToString(),
                    PositionPlayedLabel: labels.PositionLabel(ps.PositionPlayed.ToString(), SupportedLocales.Current),
                    TeamId: ps.TeamId,
                    TeamName: ps.Team?.Name ?? (ps.TeamId == match.HomeTeamId ? match.HomeTeam.Name : match.AwayTeam.Name),
                    MinutesPlayed: ps.MinutesPlayed,
                    Goals: ps.Goals,
                    Assists: ps.Assists,
                    YellowCards: ps.YellowCards,
                    RedCards: ps.RedCards,
                    FinalMps: score?.FinalMps,
                    ScoreId: score?.Id
                );
            }).ToList();

        var homeTeam = new TeamSummaryDto(
            match.HomeTeam.Id,
            match.HomeTeam.Name,
            match.HomeTeam.ShortName,
            match.HomeTeam.LogoUrl,
            match.HomeTeam.Country,
            match.HomeTeam.EloRanking
        );

        var awayTeam = new TeamSummaryDto(
            match.AwayTeam.Id,
            match.AwayTeam.Name,
            match.AwayTeam.ShortName,
            match.AwayTeam.LogoUrl,
            match.AwayTeam.Country,
            match.AwayTeam.EloRanking
        );

        return new MatchDetailDto(
            Id: match.Id,
            MatchDate: match.MatchDate,
            CompetitionName: match.Competition?.NameFor(SupportedLocales.Current) ?? string.Empty,
            CompetitionTier: match.Competition?.Tier.ToString() ?? string.Empty,
            RoundPhase: match.RoundPhase,
            IsKnockout: match.IsKnockout,
            HomeTeam: homeTeam,
            AwayTeam: awayTeam,
            HomeScore: match.HomeScore,
            AwayScore: match.AwayScore,
            PlayerPerformances: playerPerformances
        );
    }
}