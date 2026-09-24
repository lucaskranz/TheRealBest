namespace TheRealBest.API.IntegrationTests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TheRealBest.Domain.Entities;
using TheRealBest.Infrastructure;
using TheRealBest.Infrastructure.Data;

public class AppDbContextModelTests
{
    private static AppDbContext CreateContext()
    {
        // O modelo é construído sem abrir conexão com o banco
        var options = DependencyInjection
            .ConfigureDbContext(new DbContextOptionsBuilder<AppDbContext>(), "Host=localhost;Database=model_only")
            .Options;

        return new AppDbContext((DbContextOptions<AppDbContext>)options);
    }

    private static IEntityType EntityType<TEntity>(AppDbContext context) =>
        context.Model.FindEntityType(typeof(TEntity))!;

    [Theory]
    [InlineData(typeof(Player), "players")]
    [InlineData(typeof(Team), "teams")]
    [InlineData(typeof(Competition), "competitions")]
    [InlineData(typeof(Match), "matches")]
    [InlineData(typeof(MatchPlayerStats), "match_player_stats")]
    [InlineData(typeof(MatchPerformanceScore), "match_performance_scores")]
    [InlineData(typeof(SeasonRanking), "season_rankings")]
    [InlineData(typeof(ScoringWeightRule), "scoring_weights")]
    [InlineData(typeof(CompetitionTranslation), "competition_translations")]
    [InlineData(typeof(ActionTypeTranslation), "action_type_translations")]
    public void Model_EntityType_MapsToSnakeCaseTable(Type entityType, string expectedTable)
    {
        using var context = CreateContext();

        context.Model.FindEntityType(entityType)!.GetTableName().Should().Be(expectedTable);
    }

    [Fact]
    public void Model_AllColumns_UseSnakeCase()
    {
        using var context = CreateContext();

        var columns = context.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties().Select(p => p.GetColumnName()));

        columns.Should().OnlyContain(c => c == c.ToLowerInvariant());
    }

    [Fact]
    public void MatchPerformanceScore_Breakdowns_AreStoredAsJsonb()
    {
        using var context = CreateContext();
        var entity = EntityType<MatchPerformanceScore>(context);

        var action = entity.FindProperty(nameof(MatchPerformanceScore.ActionBreakdownJson))!;
        var penalty = entity.FindProperty(nameof(MatchPerformanceScore.PenaltyBreakdownJson))!;

        action.GetColumnName().Should().Be("action_breakdown");
        action.GetColumnType().Should().Be("jsonb");
        penalty.GetColumnName().Should().Be("penalty_breakdown");
        penalty.GetColumnType().Should().Be("jsonb");
    }

    [Fact]
    public void Enums_AreStoredAsStrings()
    {
        using var context = CreateContext();

        var enumProperties = context.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType.IsEnum);

        enumProperties.Should().NotBeEmpty();
        enumProperties.Should().OnlyContain(p => p.GetProviderClrType() == typeof(string));
    }

    [Fact]
    public void MatchPlayerStats_MatchAndPlayer_HaveUniqueCompositeIndex()
    {
        using var context = CreateContext();

        var index = EntityType<MatchPlayerStats>(context).GetIndexes()
            .Single(i => i.Properties.Select(p => p.Name)
                .SequenceEqual([nameof(MatchPlayerStats.MatchId), nameof(MatchPlayerStats.PlayerId)]));

        index.IsUnique.Should().BeTrue();
    }

    [Fact]
    public void Migrations_InitialCreate_IsRegistered()
    {
        using var context = CreateContext();

        context.Database.GetMigrations().Should().ContainSingle(m => m.EndsWith("_InitialCreate"));
    }
}
