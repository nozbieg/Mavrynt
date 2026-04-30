using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.Persistence.Configurations;

internal sealed class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.ToTable("feature_flags", schema: "feature_management");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(
                id => id.Value,
                guid => FeatureFlagId.From(guid).Value)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(f => f.Key)
            .HasConversion(
                key => key.Value,
                str => FeatureFlagKey.Create(str).Value)
            .HasMaxLength(FeatureFlagKey.MaxLength)
            .HasColumnName("key")
            .IsRequired();

        builder.HasIndex(f => f.Key)
            .IsUnique()
            .HasDatabaseName("ix_feature_flags_key");

        builder.Property(f => f.Name)
            .HasMaxLength(256)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(f => f.Description)
            .HasMaxLength(1024)
            .HasColumnName("description")
            .IsRequired(false);

        builder.Property(f => f.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(f => f.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);
    }
}
