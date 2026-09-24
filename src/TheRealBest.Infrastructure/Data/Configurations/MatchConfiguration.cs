namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class MatchConfiguration : EntityBaseConfiguration<Match>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("matches");

        builder.Property(m => m.ExternalApiId).HasMaxLength(ExternalIdMaxLength).IsRequired();
        builder.Property(m => m.RoundPhase).HasMaxLength(RoundPhaseMaxLength).IsRequired();
        builder.Property(m => m.MatchDate).HasColumnType("timestamp with time zone");

        builder.HasOne(m => m.Competition)
            .WithMany(c => c.Matches)
            .HasForeignKey(m => m.CompetitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.ExternalApiId).IsUnique();
        builder.HasIndex(m => new { m.CompetitionId, m.MatchDate });
        builder.HasIndex(m => m.HomeTeamId);
        builder.HasIndex(m => m.AwayTeamId);

        builder.Navigation(m => m.PlayerStats).HasField("_playerStats");
        builder.Navigation(m => m.PerformanceScores).HasField("_performanceScores");
    }
}
