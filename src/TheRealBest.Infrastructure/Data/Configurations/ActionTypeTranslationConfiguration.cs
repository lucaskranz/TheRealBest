namespace TheRealBest.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheRealBest.Domain.Entities;
using static ConfigurationConstants;

public class ActionTypeTranslationConfiguration : EntityBaseConfiguration<ActionTypeTranslation>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ActionTypeTranslation> builder)
    {
        builder.ToTable("action_type_translations");

        builder.Property(t => t.ActionKey).HasMaxLength(ShortNameMaxLength).IsRequired();
        builder.Property(t => t.Locale).HasMaxLength(LocaleMaxLength).IsRequired();
        builder.Property(t => t.Label).HasMaxLength(TranslationMaxLength).IsRequired();
        builder.Property(t => t.LabelPlural).HasMaxLength(TranslationMaxLength);

        builder.HasIndex(t => new { t.ActionKey, t.Locale }).IsUnique();
    }
}
