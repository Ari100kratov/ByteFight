using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class AddPositionsToArena : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "start_position_x",
            schema: "game",
            table: "arenas",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "start_position_y",
            schema: "game",
            table: "arenas",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "arenas_blocked_positions",
            schema: "game",
            columns: table => new
            {
                arena_id = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                x = table.Column<int>(type: "integer", nullable: false),
                y = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_arenas_blocked_positions", x => new { x.arena_id, x.id });
                table.ForeignKey(
                    name: "fk_arenas_blocked_positions_arenas_arena_id",
                    column: x => x.arena_id,
                    principalSchema: "game",
                    principalTable: "arenas",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "arenas_blocked_positions",
            schema: "game");

        migrationBuilder.DropColumn(
            name: "start_position_x",
            schema: "game",
            table: "arenas");

        migrationBuilder.DropColumn(
            name: "start_position_y",
            schema: "game",
            table: "arenas");
    }
}
