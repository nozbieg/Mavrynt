using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mavrynt.Modules.Users.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "users");

        migrationBuilder.CreateTable(
            name: "audit_events",
            schema: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                event_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: true),
                email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                source = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                metadata = table.Column<string>(type: "jsonb", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_audit_events", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            schema: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                password_hash = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_audit_events_event_type",
            schema: "users",
            table: "audit_events",
            column: "event_type");

        migrationBuilder.CreateIndex(
            name: "ix_audit_events_occurred_at",
            schema: "users",
            table: "audit_events",
            column: "occurred_at");

        migrationBuilder.CreateIndex(
            name: "ix_audit_events_user_id",
            schema: "users",
            table: "audit_events",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            schema: "users",
            table: "users",
            column: "email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "audit_events", schema: "users");
        migrationBuilder.DropTable(name: "users", schema: "users");
    }
}
