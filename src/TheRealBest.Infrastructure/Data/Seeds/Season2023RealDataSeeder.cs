namespace TheRealBest.Infrastructure.Data.Seeds;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Entities;
using TheRealBest.Infrastructure.Data;

public sealed class Season2023RealDataSeeder(
    AppDbContext dbContext,
    IIngestMatchDataUseCase ingestUseCase,
    IRecalculateSeasonRankingUseCase recalculateUseCase,
    ILogger<Season2023RealDataSeeder> logger) : IRealDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Iniciando seed de dados reais da temporada 2023/24...");

        // 1. Seed de traduÃ§Ãµes de aÃ§Ãµes (pt-BR, en, es)
        await SeedActionTranslationsAsync(cancellationToken);

        // 2. Verificar se jÃ¡ existem partidas cadastradas
        var hasMatches = await dbContext.Matches.AnyAsync(cancellationToken);
        if (hasMatches)
        {
            logger.LogInformation("Partidas jÃ¡ encontradas no banco de dados. Pulando ingestÃ£o dos relatÃ³rios de partidas.");
            return;
        }

        // 3. Ingerir as 24 partidas reais do catÃ¡logo
        var reports = RealDataCatalog.GetReports();
        logger.LogInformation("Ingerindo {Count} partidas reais com relatÃ³rios estatÃ­sticos oficiais...", reports.Count);

        int count = 0;
        foreach (var report in reports)
        {
            if (cancellationToken.IsCancellationRequested) break;

            await ingestUseCase.ExecuteAsync(report, cancellationToken);
            count++;
        }

        logger.LogInformation("IngestÃ£o de {Count} partidas finalizada. Recalculando Fair Season Score e rankings...", count);

        // 4. Recalcular os rankings da temporada 2023/24
        var summary = await recalculateUseCase.ExecuteAsync(2023, cancellationToken);

        logger.LogInformation("Seed da temporada 2023/24 concluÃ­do com sucesso! {Total} atletas avaliados, {Eligible} no ranking oficial.",
            summary.TotalRanked, summary.EligibleCount);
    }

    private async Task SeedActionTranslationsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.ActionTypeTranslations.AnyAsync(cancellationToken))
        {
            return;
        }

        var translations = new List<ActionTypeTranslation>
        {
            // Gols e AssistÃªncias
            new("Goal", "pt-BR", "Gol", "Gols"),
            new("Goal", "en", "Goal", "Goals"),
            new("Goal", "es", "Gol", "Goles"),

            new("Assist", "pt-BR", "AssistÃªncia", "AssistÃªncias"),
            new("Assist", "en", "Assist", "Assists"),
            new("Assist", "es", "Asistencia", "Asistencias"),

            new("KeyPass", "pt-BR", "Passe-chave", "Passes-chave"),
            new("KeyPass", "en", "Key Pass", "Key Passes"),
            new("KeyPass", "es", "Pase clave", "Pases clave"),

            new("Tackle", "pt-BR", "Desarme", "Desarmes"),
            new("Tackle", "en", "Tackle", "Tackles"),
            new("Tackle", "es", "Entrada", "Entradas"),

            new("Interception", "pt-BR", "InterceptaÃ§Ã£o", "InterceptaÃ§Ãµes"),
            new("Interception", "en", "Interception", "Interceptions"),
            new("Interception", "es", "IntercepciÃ³n", "Intercepciones"),

            new("CleanSheet", "pt-BR", "Jogo sem sofrer gols (Clean Sheet)", "Jogos sem sofrer gols"),
            new("CleanSheet", "en", "Clean Sheet", "Clean Sheets"),
            new("CleanSheet", "es", "Valla invicta (Clean Sheet)", "Vallas invictas"),

            new("Save", "pt-BR", "Defesa", "Defesas"),
            new("Save", "en", "Save", "Saves"),
            new("Save", "es", "Parada", "Paradas"),

            new("DribbleSuccess", "pt-BR", "Drible certo", "Dribles certos"),
            new("DribbleSuccess", "en", "Successful Dribble", "Successful Dribbles"),
            new("DribbleSuccess", "es", "Regate con Ã©xito", "Regates con Ã©xito"),

            new("BigChanceMissed", "pt-BR", "Grande chance perdida", "Grandes chances perdidas"),
            new("BigChanceMissed", "en", "Big Chance Missed", "Big Chances Missed"),
            new("BigChanceMissed", "es", "Gran ocasiÃ³n fallada", "Grandes ocasiones falladas"),

            new("YellowCard", "pt-BR", "CartÃ£o amarelo", "CartÃµes amarelos"),
            new("YellowCard", "en", "Yellow Card", "Yellow Cards"),
            new("YellowCard", "es", "Tarjeta amarilla", "Tarjetas amarillas"),

            new("RedCard", "pt-BR", "CartÃ£o vermelho", "CartÃµes vermelhos"),
            new("RedCard", "en", "Red Card", "Red Cards"),
            new("RedCard", "es", "Tarjeta roja", "Tarjetas rojas")
        };

        await dbContext.ActionTypeTranslations.AddRangeAsync(translations, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}