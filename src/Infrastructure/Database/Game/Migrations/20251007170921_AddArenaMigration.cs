using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class AddArenaMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "arena",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                grid_width = table.Column<int>(type: "integer", nullable: false),
                grid_height = table.Column<int>(type: "integer", nullable: false),
                background_asset = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                game_modes = table.Column<int[]>(type: "integer[]", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_arena", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "arena",
            schema: "game");
    }
}
