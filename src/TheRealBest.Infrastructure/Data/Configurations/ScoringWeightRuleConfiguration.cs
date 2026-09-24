namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class ScoringWeightRuleConfiguration : EntityBaseConfiguration<ScoringWeightRule>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ScoringWeightRule> builder)
    {
        builder.ToTable("scoring_weights");

        builder.Property(w => w.Position)
            .HasConversion<string>()
            .HasMaxLength(EnumMaxLength)
            .IsRequired();
        builder.Property(w => w.ActionType)
            .HasConversion<string>()
            .HasMaxLength(EnumMaxLength)
            .IsRequired();
        builder.Property(w => w.WeightValue).HasPrecision(ScorePrecision, ScoreScale);
        builder.Property(w => w.Description).HasMaxLength(DescriptionMaxLength).IsRequired();

        // Pesos versionados: uma regra por posição/ação em cada versão
        builder.HasIndex(w => new { w.Position, w.ActionType, w.Version }).IsUnique();
    }
}
