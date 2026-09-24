namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class MatchPlayerStatsConfiguration : EntityBaseConfiguration<MatchPlayerStats>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MatchPlayerStats> builder)
    {
        builder.ToTable("match_player_stats");

        builder.Property(s => s.PositionPlayed)
            .HasConversion<string>()
            .HasMaxLength(EnumMaxLength)
            .IsRequired();

        builder.Property(s => s.PassAccuracy).HasPrecision(StatPrecision, StatScale);
        builder.Property(s => s.Xg).HasPrecision(StatPrecision, StatScale);
        builder.Property(s => s.Xa).HasPrecision(StatPrecision, StatScale);

        builder.HasOne(s => s.Match)
            .WithMany(m => m.PlayerStats)
            .HasForeignKey(s => s.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Player)
            .WithMany(p => p.MatchStats)
            .HasForeignKey(s => s.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Team)
            .WithMany(t => t.PlayerStats)
            .HasForeignKey(s => s.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Um jogador tem uma única linha de estatísticas por partida
        builder.HasIndex(s => new { s.MatchId, s.PlayerId }).IsUnique();
        builder.HasIndex(s => s.PlayerId);
        builder.HasIndex(s => s.TeamId);
    }
}
