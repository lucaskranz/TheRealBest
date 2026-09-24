namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class CompetitionTranslationConfiguration : EntityBaseConfiguration<CompetitionTranslation>
{
    protected override void ConfigureEntity(EntityTypeBuilder<CompetitionTranslation> builder)
    {
        builder.ToTable("competition_translations");

        builder.Property(t => t.Locale).HasMaxLength(LocaleMaxLength).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(TranslationMaxLength).IsRequired();
        builder.Property(t => t.CountryName).HasMaxLength(CountryMaxLength);

        builder.HasIndex(t => new { t.CompetitionId, t.Locale }).IsUnique();
    }
}
