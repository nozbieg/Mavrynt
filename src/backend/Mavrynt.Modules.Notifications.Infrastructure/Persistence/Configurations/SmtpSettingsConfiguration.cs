using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mavrynt.Modules.Notifications.Infrastructure.Persistence.Configurations;

internal sealed class SmtpSettingsConfiguration : IEntityTypeConfiguration<SmtpSettings>
{
    public void Configure(EntityTypeBuilder<SmtpSettings> builder)
    {
        builder.ToTable("smtp_settings", schema: "notifications");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                guid => SmtpSettingsId.From(guid).Value)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.ProviderName)
            .HasMaxLength(256)
            .HasColumnName("provider_name")
            .IsRequired();

        builder.Property(s => s.Host)
            .HasMaxLength(256)
            .HasColumnName("host")
            .IsRequired();

        builder.Property(s => s.Port)
            .HasColumnName("port")
            .IsRequired();

        builder.Property(s => s.Username)
            .HasMaxLength(256)
            .HasColumnName("username")
            .IsRequired();

        builder.Property(s => s.ProtectedPassword)
            .HasColumnType("text")
            .HasColumnName("protected_password")
            .IsRequired();

        builder.Property(s => s.SenderEmail)
            .HasMaxLength(320)
            .HasColumnName("sender_email")
            .IsRequired();

        builder.Property(s => s.SenderName)
            .HasMaxLength(256)
            .HasColumnName("sender_name")
            .IsRequired();

        builder.Property(s => s.UseSsl)
            .HasColumnName("use_ssl")
            .IsRequired();

        builder.Property(s => s.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);
    }
}
