namespace TheRealBest.Infrastructure.Data.Seeds;

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

/// <summary>
/// Importa as partidas de <see cref="RealMatchSelection"/> com dados reais da API-Football e recalcula o ranking.
/// Retomável: partidas já no banco não gastam requisições, e a importação para (sem falhar) quando a cota diária
/// acaba — basta rodar de novo após o reset das 00:00 UTC. Com o cache de respostas, recriar o banco não custa cota.
/// </summary>
public sealed class ApiFootballRealDataSeeder(
    AppDbContext dbContext,
    IFootballDataProvider dataProvider,
    IIngestMatchDataUseCase ingestUseCase,
    IRecalculateSeasonRankingUseCase recalculateUseCase,
    ILogger<ApiFootballRealDataSeeder> logger) : IRealDataSeeder
{
    public const int SeasonYear = 2023;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedActionTranslationsAsync(cancellationToken);

        var selection = RealMatchSelection.Season2023;
        var importedIds = await ImportedFixtureIdsAsync(cancellationToken);
        var pending = selection.Where(m => !importedIds.Contains(Id(m.FixtureId))).ToList();

        logger.LogInformation(
            "Seed {Season}: {Imported}/{Total} partidas já importadas, {Pending} pendentes.",
            SeasonYear, selection.Count - pending.Count, selection.Count, pending.Count);

        var importedNow = 0;
        try
        {
            foreach (var group in pending.GroupBy(m => (m.LeagueId, m.ApiSeason)))
            {
                var fixtures = (await dataProvider.GetFixturesAsync(Id(group.Key.LeagueId), group.Key.ApiSeason, cancellationToken))
                    .ToDictionary(f => f.ExternalId);

                foreach (var seed in group)
                {
                    if (!fixtures.TryGetValue(Id(seed.FixtureId), out var fixture))
                    {
                        logger.LogWarning("Partida {FixtureId} ({Description}) não encontrada na API.", seed.FixtureId, seed.Description);
                        continue;
                    }

                    var report = await dataProvider.GetMatchReportAsync(fixture, cancellationToken);
                    await ingestUseCase.ExecuteAsync(report, cancellationToken);
                    importedNow++;
                    logger.LogInformation("Importada {FixtureId}: {Description}", seed.FixtureId, seed.Description);
                }
            }
        }
        catch (ApiFootballException ex) when (ex.Kind is ApiFootballErrorKind.DailyQuotaExceeded or ApiFootballErrorKind.RateLimited)
        {
            logger.LogWarning(
                "Cota da API-Football esgotada após {Count} partidas nesta execução. Rode o seed novamente após 00:00 UTC para continuar.",
                importedNow);
        }

        var summary = await recalculateUseCase.ExecuteAsync(SeasonYear, cancellationToken);
        logger.LogInformation(
            "Seed {Season} concluído: {ImportedNow} importadas agora, {Pending} pendentes; {Total} jogadores avaliados, {Eligible} elegíveis ao ranking.",
            SeasonYear, importedNow, pending.Count - importedNow, summary.TotalRanked, summary.EligibleCount);
    }

    private async Task<HashSet<string>> ImportedFixtureIdsAsync(CancellationToken cancellationToken) =>
        (await dbContext.Matches
            .Where(m => m.PlayerStats.Any())
            .Select(m => m.ExternalApiId)
            .ToListAsync(cancellationToken))
        .ToHashSet();

    private static string Id(int id) => id.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Traduções iniciais dos rótulos do recibo. As chaves são as de ActionCatalog (snake_case), gravadas em
    /// ActionScoreItem.ActionKey. A cobertura completa das ações fica para a Etapa 4C.
    /// </summary>
    private async Task SeedActionTranslationsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.ActionTypeTranslations.AnyAsync(cancellationToken))
        {
            return;
        }

        (string Key, string PtBr, string PtBrPlural, string En, string EnPlural, string Es, string EsPlural)[] labels =
        [
            ("goal", "Gol", "Gols", "Goal", "Goals", "Gol", "Goles"),
            ("assist", "Assistência", "Assistências", "Assist", "Assists", "Asistencia", "Asistencias"),
            ("key_pass", "Passe decisivo", "Passes decisivos", "Key pass", "Key passes", "Pase clave", "Pases clave"),
            ("tackle", "Desarme", "Desarmes", "Tackle", "Tackles", "Entrada", "Entradas"),
            ("interception", "Interceptação", "Interceptações", "Interception", "Interceptions", "Intercepción", "Intercepciones"),
            ("clean_sheet", "Clean sheet", "Clean sheets", "Clean sheet", "Clean sheets", "Portería a cero", "Porterías a cero"),
            ("save", "Defesa", "Defesas", "Save", "Saves", "Parada", "Paradas"),
            ("dribble_success", "Drible certo", "Dribles certos", "Successful dribble", "Successful dribbles", "Regate exitoso", "Regates exitosos"),
            ("big_chance_missed", "Grande chance perdida", "Grandes chances perdidas", "Big chance missed", "Big chances missed", "Ocasión clara fallada", "Ocasiones claras falladas"),
            ("yellow_card", "Cartão amarelo", "Cartões amarelos", "Yellow card", "Yellow cards", "Tarjeta amarilla", "Tarjetas amarillas"),
            ("red_card", "Cartão vermelho", "Cartões vermelhos", "Red card", "Red cards", "Tarjeta roja", "Tarjetas rojas"),
        ];

        var translations = labels.SelectMany(l => new[]
        {
            new ActionTypeTranslation(l.Key, "pt-BR", l.PtBr, l.PtBrPlural),
            new ActionTypeTranslation(l.Key, "en", l.En, l.EnPlural),
            new ActionTypeTranslation(l.Key, "es", l.Es, l.EsPlural),
        });

        await dbContext.ActionTypeTranslations.AddRangeAsync(translations, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
