using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mavrynt.Modules.Users.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddRequiresPasswordChangeAndAdminSeed : Migration
{
    // Deterministic id for the seeded local-development administrator.
    private static readonly Guid AdminUserId = new("00000000-0000-0000-0000-000000000001");

    // Hash of "Admin123!ChangeMe" produced by ASP.NET Core Identity PasswordHasher v3
    // (PBKDF2-HMAC-SHA512, 100 000 iterations). Never store the raw password anywhere
    // except developer documentation.
    private const string AdminPasswordHash =
        "AQAAAAIAAYagAAAAEAyWAyl1q3sWHKHbLtyfCO2Z67R4PIgHfnKCZQ/QK0GgXSrUGGon8gSFn/W9gRD9nQ==";

    private static readonly DateTimeOffset AdminCreatedAt =
        new(2026, 4, 30, 0, 0, 0, TimeSpan.Zero);

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "requires_password_change",
            schema: "users",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "password_changed_at",
            schema: "users",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);

        // Seed the default local-development administrator.
        // This account is intended for first-login bootstrap only.
        // Production environments must provision administrators through
        // a secure secrets/configuration flow — never via this seed.
        migrationBuilder.InsertData(
            schema: "users",
            table: "users",
            columns: new[]
            {
                "id", "email", "password_hash", "display_name",
                "status", "role", "created_at", "updated_at",
                "requires_password_change", "password_changed_at"
            },
            values: new object[]
            {
                AdminUserId,
                "admin@mavrynt.local",
                AdminPasswordHash,
                "Mavrynt Administrator",
                "Active",
                "Admin",
                AdminCreatedAt,
                null!,
                true,
                null!
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "users",
            table: "users",
            keyColumn: "id",
            keyValue: AdminUserId);

        migrationBuilder.DropColumn(
            name: "password_changed_at",
            schema: "users",
            table: "users");

        migrationBuilder.DropColumn(
            name: "requires_password_change",
            schema: "users",
            table: "users");
    }
}
