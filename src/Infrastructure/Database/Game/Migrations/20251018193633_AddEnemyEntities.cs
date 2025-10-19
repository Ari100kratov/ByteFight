using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class AddEnemyEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "enemies",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemies", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "arena_enemies",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                arena_id = table.Column<Guid>(type: "uuid", nullable: false),
                enemy_id = table.Column<Guid>(type: "uuid", nullable: false),
                position_x = table.Column<int>(type: "integer", nullable: false),
                position_y = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_arena_enemies", x => x.id);
                table.ForeignKey(
                    name: "fk_arena_enemies_arenas_arena_id",
                    column: x => x.arena_id,
                    principalSchema: "game",
                    principalTable: "arenas",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_arena_enemies_enemies_enemy_id",
                    column: x => x.enemy_id,
                    principalSchema: "game",
                    principalTable: "enemies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "enemy_assets",
            schema: "game",
            columns: table => new
            {
                enemy_id = table.Column<Guid>(type: "uuid", nullable: false),
                action_type = table.Column<int>(type: "integer", nullable: false),
                url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemy_assets", x => new { x.enemy_id, x.action_type });
                table.ForeignKey(
                    name: "fk_enemy_assets_enemies_enemy_id",
                    column: x => x.enemy_id,
                    principalSchema: "game",
                    principalTable: "enemies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "enemy_stats",
            schema: "game",
            columns: table => new
            {
                enemy_id = table.Column<Guid>(type: "uuid", nullable: false),
                stat_type = table.Column<int>(type: "integer", nullable: false),
                value = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemy_stats", x => new { x.enemy_id, x.stat_type });
                table.ForeignKey(
                    name: "fk_enemy_stats_enemies_enemy_id",
                    column: x => x.enemy_id,
                    principalSchema: "game",
                    principalTable: "enemies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_arena_enemies_arena_id",
            schema: "game",
            table: "arena_enemies",
            column: "arena_id");

        migrationBuilder.CreateIndex(
            name: "ix_arena_enemies_enemy_id",
            schema: "game",
            table: "arena_enemies",
            column: "enemy_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "arena_enemies",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemy_assets",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemy_stats",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemies",
            schema: "game");
    }
}
