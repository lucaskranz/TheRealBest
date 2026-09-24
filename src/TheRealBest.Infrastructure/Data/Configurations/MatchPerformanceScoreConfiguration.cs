namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class MatchPerformanceScoreConfiguration : EntityBaseConfiguration<MatchPerformanceScore>
{
    private const string EmptyJsonArray = "'[]'::jsonb";

    protected override void ConfigureEntity(EntityTypeBuilder<MatchPerformanceScore> builder)
    {
        builder.ToTable("match_performance_scores");

        builder.Property(s => s.PositionEvaluated)
            .HasConversion<string>()
            .HasMaxLength(EnumMaxLength)
            .IsRequired();

        // Extrato auditável: armazenado em JSONB e renderizado sem recálculo no frontend
        builder.Property(s => s.ActionBreakdownJson)
            .HasColumnName("action_breakdown")
            .HasColumnType("jsonb")
            .HasDefaultValueSql(EmptyJsonArray)
            .IsRequired();
        builder.Property(s => s.PenaltyBreakdownJson)
            .HasColumnName("penalty_breakdown")
            .HasColumnType("jsonb")
            .HasDefaultValueSql(EmptyJsonArray)
            .IsRequired();

        builder.Property(s => s.BaseScore).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.SubtotalRaw).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.TournamentMultiplier).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.OpponentMultiplier).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.ClutchMultiplier).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.ContextMultiplierCombined).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.MinutesFactor).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.FinalMps).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(s => s.CalculatedAt).HasColumnType("timestamp with time zone");

        builder.HasOne(s => s.Match)
            .WithMany(m => m.PerformanceScores)
            .HasForeignKey(s => s.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Player)
            .WithMany(p => p.PerformanceScores)
            .HasForeignKey(s => s.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.MatchPlayerStats)
            .WithOne(st => st.PerformanceScore)
            .HasForeignKey<MatchPerformanceScore>(s => s.MatchPlayerStatsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.PlayerId, s.MatchId });
        builder.HasIndex(s => new { s.PlayerId, s.AlgorithmVersion });
        builder.HasIndex(s => s.MatchId);
    }
}
