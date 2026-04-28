using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Enums;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mavrynt.Modules.Users.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", schema: "users");

        // ── Primary key ──────────────────────────────────────────────────────
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                // DB data is trusted; Result.Value throws on corrupt Guid.Empty
                guid => UserId.From(guid).Value)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // ── Email ────────────────────────────────────────────────────────────
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                str => Email.Create(str).Value)
            .HasMaxLength(Email.MaxLength)
            .HasColumnName("email")
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email");

        // ── Password hash ─────────────────────────────────────────────────────
        builder.Property(u => u.PasswordHash)
            .HasConversion(
                ph => ph.Value,
                str => PasswordHash.Create(str).Value)
            .HasMaxLength(PasswordHash.MaxLength)
            .HasColumnName("password_hash")
            .IsRequired();

        // ── Display name (nullable) ───────────────────────────────────────────
        var displayNameConverter = new ValueConverter<UserDisplayName, string>(
            dn => dn.Value,
            str => UserDisplayName.Create(str).Value);

        builder.Property(u => u.DisplayName)
            .HasConversion(displayNameConverter!)
            .HasMaxLength(UserDisplayName.MaxLength)
            .HasColumnName("display_name")
            .IsRequired(false);

        // ── Status enum → string ──────────────────────────────────────────────
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnName("status")
            .IsRequired();

        // ── Role enum → string ────────────────────────────────────────────────
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnName("role")
            .IsRequired();

        // ── Timestamps ────────────────────────────────────────────────────────
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);
    }
}
