using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mavrynt.Modules.Notifications.Infrastructure.Persistence.Configurations;

internal sealed class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.ToTable("email_templates", schema: "notifications");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                guid => EmailTemplateId.From(guid).Value)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.Key)
            .HasConversion(
                key => key.Value,
                str => EmailTemplateKey.Create(str).Value)
            .HasMaxLength(EmailTemplateKey.MaxLength)
            .HasColumnName("template_key")
            .IsRequired();

        builder.HasIndex(t => t.Key)
            .IsUnique()
            .HasDatabaseName("ix_email_templates_template_key");

        builder.Property(t => t.DisplayName)
            .HasMaxLength(256)
            .HasColumnName("display_name")
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1024)
            .HasColumnName("description")
            .IsRequired(false);

        builder.Property(t => t.SubjectTemplate)
            .HasMaxLength(512)
            .HasColumnName("subject_template")
            .IsRequired();

        builder.Property(t => t.HtmlBodyTemplate)
            .HasColumnType("text")
            .HasColumnName("html_body_template")
            .IsRequired();

        builder.Property(t => t.TextBodyTemplate)
            .HasColumnType("text")
            .HasColumnName("text_body_template")
            .IsRequired(false);

        builder.Property(t => t.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);
    }
}
