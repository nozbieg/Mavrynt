using Mavrynt.Modules.Users.Infrastructure.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mavrynt.Modules.Users.Infrastructure.Persistence.Configurations;

internal sealed class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("audit_events", schema: "users");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(a => a.EventType)
            .HasMaxLength(64)
            .HasColumnName("event_type")
            .IsRequired();

        builder.Property(a => a.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.Property(a => a.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);

        builder.Property(a => a.Email)
            .HasMaxLength(254)
            .HasColumnName("email")
            .IsRequired(false);

        builder.Property(a => a.Source)
            .HasMaxLength(64)
            .HasColumnName("source")
            .IsRequired(false);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45) // IPv6 max length
            .HasColumnName("ip_address")
            .IsRequired(false);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(512)
            .HasColumnName("user_agent")
            .IsRequired(false);

        builder.Property(a => a.Metadata)
            .HasColumnType("jsonb")
            .HasColumnName("metadata")
            .IsRequired(false);

        builder.HasIndex(a => a.OccurredAt)
            .HasDatabaseName("ix_audit_events_occurred_at");

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("ix_audit_events_user_id");

        builder.HasIndex(a => a.EventType)
            .HasDatabaseName("ix_audit_events_event_type");
    }
}
