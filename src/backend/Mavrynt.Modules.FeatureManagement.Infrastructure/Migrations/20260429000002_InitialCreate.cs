using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "feature_management");

        migrationBuilder.CreateTable(
            name: "feature_flags",
            schema: "feature_management",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_feature_flags", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_feature_flags_key",
            schema: "feature_management",
            table: "feature_flags",
            column: "key",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "feature_flags", schema: "feature_management");
    }
}
