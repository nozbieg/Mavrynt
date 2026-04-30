using Mavrynt.Modules.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mavrynt.Modules.Audit.Infrastructure.Persistence.Configurations;

internal sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("audit_log_entries", schema: "audit");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(a => a.ActorUserId)
            .HasColumnName("actor_user_id")
            .IsRequired(false);

        builder.Property(a => a.Action)
            .HasMaxLength(128)
            .HasColumnName("action")
            .IsRequired();

        builder.Property(a => a.ResourceType)
            .HasMaxLength(128)
            .HasColumnName("resource_type")
            .IsRequired();

        builder.Property(a => a.ResourceId)
            .HasMaxLength(256)
            .HasColumnName("resource_id")
            .IsRequired(false);

        builder.Property(a => a.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.Property(a => a.MetadataJson)
            .HasColumnType("jsonb")
            .HasColumnName("metadata_json")
            .IsRequired(false);

        builder.HasIndex(a => a.ActorUserId)
            .HasDatabaseName("ix_audit_log_entries_actor_user_id");

        builder.HasIndex(a => new { a.ResourceType, a.ResourceId })
            .HasDatabaseName("ix_audit_log_entries_resource");

        builder.HasIndex(a => a.OccurredAt)
            .HasDatabaseName("ix_audit_log_entries_occurred_at");

        builder.HasIndex(a => a.Action)
            .HasDatabaseName("ix_audit_log_entries_action");
    }
}
