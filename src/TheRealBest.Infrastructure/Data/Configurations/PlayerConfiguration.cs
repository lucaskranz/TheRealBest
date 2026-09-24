namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class PlayerConfiguration : EntityBaseConfiguration<Player>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("players");

        builder.Property(p => p.ExternalApiId).HasMaxLength(ExternalIdMaxLength).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(NameMaxLength).IsRequired();
        builder.Property(p => p.Nationality).HasMaxLength(CountryMaxLength).IsRequired();
        builder.Property(p => p.PhotoUrl).HasMaxLength(UrlMaxLength).IsRequired();
        builder.Property(p => p.PrimaryPosition)
            .HasConversion<string>()
            .HasMaxLength(EnumMaxLength)
            .IsRequired();

        builder.HasIndex(p => p.ExternalApiId).IsUnique();
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.PrimaryPosition);

        builder.Navigation(p => p.MatchStats).HasField("_matchStats");
        builder.Navigation(p => p.PerformanceScores).HasField("_performanceScores");
        builder.Navigation(p => p.SeasonRankings).HasField("_seasonRankings");
    }
}
