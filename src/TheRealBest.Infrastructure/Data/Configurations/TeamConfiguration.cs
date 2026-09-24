namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class TeamConfiguration : EntityBaseConfiguration<Team>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("teams");

        builder.Property(t => t.ExternalApiId).HasMaxLength(ExternalIdMaxLength).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(NameMaxLength).IsRequired();
        builder.Property(t => t.ShortName).HasMaxLength(ShortNameMaxLength).IsRequired();
        builder.Property(t => t.LogoUrl).HasMaxLength(UrlMaxLength).IsRequired();
        builder.Property(t => t.Country).HasMaxLength(CountryMaxLength).IsRequired();

        builder.HasIndex(t => t.ExternalApiId).IsUnique();

        builder.Navigation(t => t.HomeMatches).HasField("_homeMatches");
        builder.Navigation(t => t.AwayMatches).HasField("_awayMatches");
        builder.Navigation(t => t.PlayerStats).HasField("_playerStats");
    }
}
