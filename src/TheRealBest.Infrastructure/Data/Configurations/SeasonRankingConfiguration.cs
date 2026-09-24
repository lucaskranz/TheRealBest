namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class SeasonRankingConfiguration : EntityBaseConfiguration<SeasonRanking>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SeasonRanking> builder)
    {
        builder.ToTable("season_rankings");

        builder.Property(r => r.MpsAverage).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(r => r.MpsSumWeighted).HasPrecision(AggregatePrecision, ScoreScale);
        builder.Property(r => r.PresenceFactor).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(r => r.FssScore).HasPrecision(AggregatePrecision, ScoreScale);
        builder.Property(r => r.ClutchIndex).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(r => r.Top5MatchesJson)
            .HasColumnName("top_5_matches")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb")
            .IsRequired();
        builder.Property(r => r.RecalculatedAt).HasColumnType("timestamp with time zone");

        builder.HasOne(r => r.Player)
            .WithMany(p => p.SeasonRankings)
            .HasForeignKey(r => r.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Um registro por jogador por temporada (base para o upsert do ranking)
        builder.HasIndex(r => new { r.PlayerId, r.SeasonYear }).IsUnique();
        builder.HasIndex(r => new { r.SeasonYear, r.OverallRank });
        builder.HasIndex(r => new { r.SeasonYear, r.PositionRank });
    }
}
