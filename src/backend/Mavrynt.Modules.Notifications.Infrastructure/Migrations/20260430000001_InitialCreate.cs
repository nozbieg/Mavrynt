using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mavrynt.Modules.Notifications.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "notifications");

        migrationBuilder.CreateTable(
            name: "smtp_settings",
            schema: "notifications",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                provider_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                host = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                port = table.Column<int>(type: "integer", nullable: false),
                username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                protected_password = table.Column<string>(type: "text", nullable: false),
                sender_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                sender_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                use_ssl = table.Column<bool>(type: "boolean", nullable: false),
                is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_smtp_settings", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "email_templates",
            schema: "notifications",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                template_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                subject_template = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                html_body_template = table.Column<string>(type: "text", nullable: false),
                text_body_template = table.Column<string>(type: "text", nullable: true),
                is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_email_templates", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_email_templates_template_key",
            schema: "notifications",
            table: "email_templates",
            column: "template_key",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "smtp_settings", schema: "notifications");
        migrationBuilder.DropTable(name: "email_templates", schema: "notifications");
    }
}
