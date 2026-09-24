namespace TheRealBest.Application.UseCases;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.DTOs;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.ValueObjects;

public sealed class IngestMatchDataUseCase(
    ICompetitionRepository competitionRepository,
    ITeamRepository teamRepository,
    IPlayerRepository playerRepository,
    IMatchRepository matchRepository,
    IUnitOfWork unitOfWork,
    IScoringEngine scoringEngine,
    IClubEloProvider clubEloProvider,
    ILogger<IngestMatchDataUseCase> logger) : IIngestMatchDataUseCase
{
    public async Task<IngestionResultDto> ExecuteAsync(ExternalMatchReport report, CancellationToken cancellationToken = default)
    {
        var fixture = report.Fixture;
        logger.LogInformation("Iniciando ingestÃ£o da partida {FixtureId}: {Home} vs {Away}",
            fixture.ExternalId, fixture.HomeTeam.Name, fixture.AwayTeam.Name);

        // 1. Verificar se a partida jÃ¡ foi importada
        var existingByExtId = await matchRepository.GetByExternalIdAsync(fixture.ExternalId, cancellationToken);

        if (existingByExtId is not null && existingByExtId.PlayerStats.Count > 0)
        {
            logger.LogInformation("Partida {FixtureId} jÃ¡ importada anteriormente com {Count} jogadores. Pulando.",
                fixture.ExternalId, existingByExtId.PlayerStats.Count);
            return new IngestionResultDto(existingByExtId.Id, fixture.ExternalId, existingByExtId.PlayerStats.Count, existingByExtId.PlayerStats.Count, AlreadyExists: true);
        }

        // 2. Garantir CompetiÃ§Ã£o
        var competition = await competitionRepository.GetByExternalIdAndSeasonAsync(fixture.Competition.ExternalId, fixture.Competition.SeasonYear, cancellationToken);
        if (competition is null)
        {
            var multiplier = fixture.Competition.Tier switch
            {
                Domain.Enums.CompetitionTier.WorldCup => 1.40m,
                Domain.Enums.CompetitionTier.UclKnockout => 1.35m,
                Domain.Enums.CompetitionTier.TopLeague => 1.10m,
                Domain.Enums.CompetitionTier.DomesticCup => 1.05m,
                Domain.Enums.CompetitionTier.InternationalContinental => 1.25m,
                _ => 1.00m
            };

            competition = new Competition(
                fixture.Competition.ExternalId,
                fixture.Competition.Name,
                fixture.Competition.Country,
                fixture.Competition.Tier,
                multiplier,
                fixture.Competition.SeasonYear);

            foreach (var (locale, name) in fixture.Competition.LocalizedNames ?? new Dictionary<string, string>())
            {
                competition.AddTranslation(locale, name);
            }

            await competitionRepository.AddAsync(competition, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // 3. Garantir times
        var homeTeam = await EnsureTeamAsync(fixture.HomeTeam, cancellationToken);
        var awayTeam = await EnsureTeamAsync(fixture.AwayTeam, cancellationToken);

        // 4. Criar a partida
        var match = existingByExtId ?? new Match(
            fixture.ExternalId,
            competition.Id,
            homeTeam.Id,
            awayTeam.Id,
            fixture.RoundPhase,
            fixture.KickoffUtc,
            fixture.IsKnockout,
            fixture.HomeScore,
            fixture.AwayScore);

        if (existingByExtId is null)
        {
            await matchRepository.AddAsync(match, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // 5. Elo dos clubes na data do jogo (seleções não têm rating no ClubElo: multiplicador neutro)
        if (match.HomeEloRating is null && match.AwayEloRating is null && IsClubCompetition(competition.Tier))
        {
            var matchDate = DateOnly.FromDateTime(match.MatchDate);
            match.SetEloRatings(
                await clubEloProvider.GetEloAsync(homeTeam.Name, matchDate, cancellationToken),
                await clubEloProvider.GetEloAsync(awayTeam.Name, matchDate, cancellationToken));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        int playersProcessed = 0;
        int scoresCalculated = 0;

        // 6. Ingerir jogadores e estatÃ­sticas
        foreach (var perf in report.Performances)
        {
            var player = await EnsurePlayerAsync(perf.Player, perf.Stats.Position, cancellationToken);
            var isHome = perf.TeamExternalId == fixture.HomeTeam.ExternalId;
            var playerTeamId = isHome ? homeTeam.Id : awayTeam.Id;

            var stats = MatchPlayerStats.FromStatLine(match.Id, player.Id, playerTeamId, perf.Stats);
            await matchRepository.AddPlayerStatsAsync(stats, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var matchContext = new MatchContext(
                competition.Tier,
                match.IsKnockout,
                match.RoundPhase,
                OpponentEloRanking: isHome ? match.AwayEloRating : match.HomeEloRating,
                TeamScore: isHome ? match.HomeScore.GetValueOrDefault() : match.AwayScore.GetValueOrDefault(),
                OpponentScore: isHome ? match.AwayScore.GetValueOrDefault() : match.HomeScore.GetValueOrDefault());

            var receipt = scoringEngine.CalculateMatchScore(stats, matchContext);

            var actionBreakdownJson = JsonSerializer.Serialize(receipt.ActionBreakdown);
            var penaltyBreakdownJson = JsonSerializer.Serialize(receipt.PenaltyBreakdown);

            var performanceScore = new MatchPerformanceScore(
                match.Id,
                player.Id,
                stats.Id,
                receipt.PositionEvaluated,
                receipt.BaseScore,
                actionBreakdownJson,
                penaltyBreakdownJson,
                receipt.SubtotalRaw,
                receipt.PositionBaseline,
                receipt.ContextMultiplier.TournamentMultiplier,
                receipt.ContextMultiplier.OpponentMultiplier,
                receipt.ContextMultiplier.ClutchMultiplier,
                receipt.ContextMultiplier.Combined,
                receipt.MinutesFactor,
                receipt.FinalMps,
                receipt.AlgorithmVersion,
                receipt.CalculatedAt,
                receipt.CountsTowardsSeason);

            await matchRepository.AddPerformanceScoreAsync(performanceScore, cancellationToken);

            playersProcessed++;
            scoresCalculated++;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("IngestÃ£o concluÃ­da para partida {FixtureId}. Processados {Players} jogadores.",
            fixture.ExternalId, playersProcessed);

        return new IngestionResultDto(match.Id, fixture.ExternalId, playersProcessed, scoresCalculated);
    }

    private static bool IsClubCompetition(Domain.Enums.CompetitionTier tier) =>
        tier is not (Domain.Enums.CompetitionTier.WorldCup or Domain.Enums.CompetitionTier.InternationalContinental);

    private async Task<Team> EnsureTeamAsync(ExternalTeam external, CancellationToken cancellationToken)
    {
        var team = await teamRepository.GetByExternalIdAsync(external.ExternalId, cancellationToken);
        if (team is not null) return team;

        team = new Team(external.ExternalId, external.Name, external.Name, external.LogoUrl, "Unknown", 1500);
        await teamRepository.AddAsync(team, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return team;
    }

    private async Task<Player> EnsurePlayerAsync(ExternalPlayer external, Domain.Enums.PlayerPosition defaultPosition, CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetByExternalIdAsync(external.ExternalId, cancellationToken);
        if (player is not null) return player;

        player = new Player(
            external.ExternalId,
            external.Name,
            "Unknown",
            null,
            external.PhotoUrl,
            defaultPosition);

        await playerRepository.AddAsync(player, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return player;
    }
}