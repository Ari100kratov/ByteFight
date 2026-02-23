using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
public partial class AddGameActionLogEntry : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "game_action_log_entries",
            schema: "game_runtime",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                actor_id = table.Column<Guid>(type: "uuid", nullable: false),
                action_type = table.Column<int>(type: "integer", nullable: false),
                info = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                turn_index = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                target_id = table.Column<Guid>(type: "uuid", nullable: true),
                damage = table.Column<decimal>(type: "numeric", nullable: true),
                facing_direction = table.Column<int>(type: "integer", nullable: true),
                target_hp_current = table.Column<decimal>(type: "numeric", nullable: true),
                target_hp_max = table.Column<decimal>(type: "numeric", nullable: true),
                walk_log_entry_facing_direction = table.Column<int>(type: "integer", nullable: true),
                to_x = table.Column<int>(type: "integer", nullable: true),
                to_y = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_game_action_log_entries", x => x.id);
                table.ForeignKey(
                    name: "fk_game_action_log_entries_game_sessions_session_id",
                    column: x => x.session_id,
                    principalSchema: "game_runtime",
                    principalTable: "game_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_game_action_log_entries_session_id",
            schema: "game_runtime",
            table: "game_action_log_entries",
            column: "session_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "game_action_log_entries",
            schema: "game_runtime");
    }
}
