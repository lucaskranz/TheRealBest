namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class CompetitionConfiguration : EntityBaseConfiguration<Competition>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Competition> builder)
    {
        builder.ToTable("competitions");

        builder.Property(c => c.ExternalApiId).HasMaxLength(ExternalIdMaxLength).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(NameMaxLength).IsRequired();
        builder.Property(c => c.Country).HasMaxLength(CountryMaxLength).IsRequired();
        builder.Property(c => c.Tier)
            .HasConversion<string>()
            .HasMaxLength(EnumMaxLength)
            .IsRequired();
        builder.Property(c => c.TournamentMultiplier).HasPrecision(ScorePrecision, ScoreScale);

        // A mesma competição externa existe uma vez por temporada
        builder.HasIndex(c => new { c.ExternalApiId, c.SeasonYear }).IsUnique();

        builder.HasMany(c => c.Translations)
            .WithOne(t => t.Competition)
            .HasForeignKey(t => t.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Matches).HasField("_matches");
        // Sempre carregadas junto com a competição (são poucas linhas) para Competition.NameFor(locale)
        builder.Navigation(c => c.Translations).HasField("_translations").AutoInclude();
    }
}
