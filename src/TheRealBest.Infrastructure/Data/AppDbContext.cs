namespace TheRealBest.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using TheRealBest.Domain.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Competition> Competitions => Set<Competition>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchPlayerStats> MatchPlayerStats => Set<MatchPlayerStats>();
    public DbSet<MatchPerformanceScore> MatchPerformanceScores => Set<MatchPerformanceScore>();
    public DbSet<SeasonRanking> SeasonRankings => Set<SeasonRanking>();
    public DbSet<ScoringWeightRule> ScoringWeights => Set<ScoringWeightRule>();
    public DbSet<CompetitionTranslation> CompetitionTranslations => Set<CompetitionTranslation>();
    public DbSet<ActionTypeTranslation> ActionTypeTranslations => Set<ActionTypeTranslation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // unaccent: busca por nome sem diferenciar acentos ("vinicius" encontra "Vinícius")
        modelBuilder.HasPostgresExtension("unaccent");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        TouchModifiedEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        TouchModifiedEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Garante updated_at preenchido em entidades alteradas sem passar por MarkUpdated() do domínio.
    /// </summary>
    private void TouchModifiedEntities()
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Modified && !entry.Property(e => e.UpdatedAt).IsModified)
            {
                entry.Entity.MarkUpdated();
            }
        }
    }
}
