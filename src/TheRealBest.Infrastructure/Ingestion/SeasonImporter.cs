namespace TheRealBest.Infrastructure.Ingestion;

using System.Globalization;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Ingestion;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <param name="Season">Ano de início da temporada (2023 = 2023/24).</param>
/// <param name="CompetitionId">Id da competição na API-Football; nulo = todas as do escopo da temporada.</param>
/// <param name="MaxMatches">Teto de partidas importadas nesta execução, para testes que não podem gastar muita cota.</param>
public sealed record SeasonImportRequest(int Season, int? CompetitionId = null, int? MaxMatches = null);

public enum SeasonImportStop
{
    /// <summary>Todas as partidas encerradas do escopo foram processadas.</summary>
    Completed,

    /// <summary>Atingiu o teto de partidas pedido.</summary>
    LimitReached,

    /// <summary>Cota diária ou limite por minuto esgotado: rodar de novo depois do reset (00:00 UTC).</summary>
    QuotaExhausted,

    /// <summary>A temporada não está disponível no plano contratado.</summary>
    SeasonNotInPlan,
}

/// <param name="Listed">Partidas da competição devolvidas pela API.</param>
/// <param name="Finished">Encerradas e pertencentes à temporada.</param>
/// <param name="AlreadyImported">Já estavam no banco antes desta execução.</param>
/// <param name="WithoutPlayerData">Importadas, mas a fonte não tinha estatísticas de jogador (cobertura limitada).</param>
/// <param name="NotReturned">Pedidas em lote e ausentes na resposta; ficam pendentes para a próxima execução.</param>
public sealed record CompetitionImportLine(
    int LeagueId,
    int ApiSeason,
    string Name,
    int Listed,
    int Finished,
    int AlreadyImported,
    int Imported,
    int WithoutPlayerData,
    int NotReturned)
{
    public int Pending => Finished - AlreadyImported - Imported - WithoutPlayerData;
}

public sealed record SeasonImportSummary(int Season, SeasonImportStop Stop, IReadOnlyList<CompetitionImportLine> Competitions)
{
    public int Imported => Competitions.Sum(c => c.Imported + c.WithoutPlayerData);
}

public interface ISeasonImporter
{
    Task<SeasonImportSummary> ImportAsync(SeasonImportRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Importa todas as partidas encerradas de uma temporada (ou de uma competição dela) e recalcula o ranking.
/// Retomável: partidas já gravadas não gastam requisições, e a execução para sem falhar quando a cota acaba.
/// Custo: 1 requisição por competição (listagem) + os detalhes das partidas, em lotes quando o plano permite.
/// </summary>
public sealed class SeasonImporter(
    IFootballDataProvider dataProvider,
    IMatchRepository matchRepository,
    IIngestMatchDataUseCase ingestUseCase,
    IRecalculateSeasonRankingUseCase recalculateUseCase,
    ILogger<SeasonImporter> logger) : ISeasonImporter
{
    /// <summary>Partidas buscadas e gravadas por rodada: o tamanho do lote da API.</summary>
    public const int ChunkSize = ApiFootballDataProvider.MaxFixturesPerBatch;

    public async Task<SeasonImportSummary> ImportAsync(SeasonImportRequest request, CancellationToken cancellationToken = default)
    {
        var scope = ApiFootballSeasonScope.For(request.Season)
            .Where(e => request.CompetitionId is null || e.LeagueId == request.CompetitionId)
            .ToList();

        if (scope.Count == 0)
        {
            throw new ArgumentException(
                $"Competition {request.CompetitionId} is not in the scope of season {request.Season}.", nameof(request));
        }

        var lines = new List<CompetitionImportLine>();
        var remaining = request.MaxMatches ?? int.MaxValue;
        var stop = SeasonImportStop.Completed;

        foreach (var entry in scope)
        {
            if (remaining <= 0)
            {
                stop = SeasonImportStop.LimitReached;
                break;
            }

            var progress = new Progress(entry);
            lines.Add(progress.ToLine());

            try
            {
                remaining -= await ImportCompetitionAsync(request.Season, progress, remaining, cancellationToken);
            }
            catch (ApiFootballException ex) when (ex.Kind is ApiFootballErrorKind.DailyQuotaExceeded or ApiFootballErrorKind.RateLimited)
            {
                logger.LogWarning("Cota da API-Football esgotada: {Message}. Rode de novo após 00:00 UTC para continuar.", ex.Message);
                stop = SeasonImportStop.QuotaExhausted;
            }
            catch (ApiFootballException ex) when (ex.Kind is ApiFootballErrorKind.SeasonNotAvailable)
            {
                logger.LogError("O plano atual não cobre esta temporada: {Message}", ex.Message);
                stop = SeasonImportStop.SeasonNotInPlan;
            }
            finally
            {
                lines[^1] = progress.ToLine();
            }

            if (stop is not SeasonImportStop.Completed)
            {
                break;
            }
        }

        if (stop == SeasonImportStop.Completed && remaining <= 0 && request.MaxMatches is not null)
        {
            stop = SeasonImportStop.LimitReached;
        }

        var summary = new SeasonImportSummary(request.Season, stop, lines);
        LogSummary(summary);

        if (summary.Imported > 0)
        {
            var ranking = await recalculateUseCase.ExecuteAsync(request.Season, cancellationToken);
            logger.LogInformation(
                "Ranking {Season} recalculado: {Total} jogadores avaliados, {Eligible} elegíveis.",
                Season(request.Season), ranking.TotalRanked, ranking.EligibleCount);
        }

        return summary;
    }

    /// <returns>Partidas importadas nesta competição.</returns>
    private async Task<int> ImportCompetitionAsync(int season, Progress progress, int limit, CancellationToken cancellationToken)
    {
        var entry = progress.Entry;
        var listed = await dataProvider.GetFixturesAsync(Id(entry.LeagueId), entry.ApiSeason, cancellationToken);
        progress.Listed = listed.Count;
        progress.Name = listed.FirstOrDefault()?.Competition.Name ?? progress.Name;

        // Torneios de seleções: só as partidas da temporada pedida (a Copa de 2026 começa em junho, fim de 2025/26)
        var finished = listed.Where(f => f.IsFinished && f.Competition.SeasonYear == season).ToList();
        progress.Finished = finished.Count;

        var existing = await matchRepository.GetExistingExternalIdsAsync(finished.Select(f => f.ExternalId).ToList(), cancellationToken);
        progress.AlreadyImported = existing.Count;

        var pending = finished.Where(f => !existing.Contains(f.ExternalId)).Take(limit).ToList();
        logger.LogInformation(
            "{Competition} {Season}: {Finished} partidas encerradas, {Existing} já importadas, {Pending} a importar agora.",
            progress.Name, Season(season), finished.Count, existing.Count, pending.Count);

        var importedHere = 0;
        foreach (var chunk in pending.Chunk(ChunkSize))
        {
            var reports = await dataProvider.GetMatchReportsAsync(chunk, cancellationToken);
            progress.NotReturned += chunk.Length - reports.Count;

            foreach (var report in reports)
            {
                await IngestAsync(report, cancellationToken);
                if (report.Performances.Count == 0)
                {
                    progress.WithoutPlayerData++;
                }
                else
                {
                    progress.Imported++;
                }

                importedHere++;
            }

            logger.LogInformation("{Competition}: {Done}/{Total} importadas.", progress.Name, importedHere, pending.Count);
        }

        return importedHere;
    }

    private async Task IngestAsync(ExternalMatchReport report, CancellationToken cancellationToken)
    {
        try
        {
            await ingestUseCase.ExecuteAsync(report, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Para a execução: o contexto do banco pode ter ficado inconsistente. A partida não foi gravada (gravação atômica).
            logger.LogError(ex, "Falha ao gravar a partida {FixtureId} ({Home} × {Away}).",
                report.Fixture.ExternalId, report.Fixture.HomeTeam.Name, report.Fixture.AwayTeam.Name);
            throw;
        }
    }

    private void LogSummary(SeasonImportSummary summary)
    {
        foreach (var line in summary.Competitions)
        {
            logger.LogInformation(
                "  {Competition,-28} listadas {Listed,4} | encerradas {Finished,4} | já no banco {Existing,4} | importadas {Imported,4} | sem dados de jogador {Empty,3} | pendentes {Pending,4}",
                line.Name, line.Listed, line.Finished, line.AlreadyImported, line.Imported, line.WithoutPlayerData, line.Pending);
        }

        logger.LogInformation("Importação {Season} terminou ({Stop}): {Imported} partidas gravadas nesta execução.",
            Season(summary.Season), summary.Stop, summary.Imported);
    }

    private static string Id(int id) => id.ToString(CultureInfo.InvariantCulture);

    private static string Season(int startYear) => $"{startYear}/{(startYear + 1) % 100:00}";

    private sealed class Progress(SeasonScopeEntry entry)
    {
        public SeasonScopeEntry Entry { get; } = entry;
        public string Name { get; set; } = $"league {entry.LeagueId}";
        public int Listed { get; set; }
        public int Finished { get; set; }
        public int AlreadyImported { get; set; }
        public int Imported { get; set; }
        public int WithoutPlayerData { get; set; }
        public int NotReturned { get; set; }

        public CompetitionImportLine ToLine() =>
            new(Entry.LeagueId, Entry.ApiSeason, Name, Listed, Finished, AlreadyImported, Imported, WithoutPlayerData, NotReturned);
    }
}
