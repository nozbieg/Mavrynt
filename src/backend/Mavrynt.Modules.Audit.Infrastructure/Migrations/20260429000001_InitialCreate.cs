using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mavrynt.Modules.Audit.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "audit");

        migrationBuilder.CreateTable(
            name: "audit_log_entries",
            schema: "audit",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                actor_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                action = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                resource_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                resource_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                metadata_json = table.Column<string>(type: "jsonb", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_audit_log_entries", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_audit_log_entries_action",
            schema: "audit",
            table: "audit_log_entries",
            column: "action");

        migrationBuilder.CreateIndex(
            name: "ix_audit_log_entries_actor_user_id",
            schema: "audit",
            table: "audit_log_entries",
            column: "actor_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_audit_log_entries_occurred_at",
            schema: "audit",
            table: "audit_log_entries",
            column: "occurred_at");

        migrationBuilder.CreateIndex(
            name: "ix_audit_log_entries_resource",
            schema: "audit",
            table: "audit_log_entries",
            columns: new[] { "resource_type", "resource_id" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "audit_log_entries", schema: "audit");
    }
}
