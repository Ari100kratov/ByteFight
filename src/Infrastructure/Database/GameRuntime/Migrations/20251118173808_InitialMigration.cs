using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "game_runtime");

        migrationBuilder.CreateTable(
            name: "game_session_participants",
            schema: "game_runtime",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                unit_type = table.Column<int>(type: "integer", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: true),
                joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_game_session_participants", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "game_sessions",
            schema: "game_runtime",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                mode = table.Column<int>(type: "integer", nullable: false),
                arena_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_ids = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ended_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                total_turns = table.Column<int>(type: "integer", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                error_message = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                result_outcome = table.Column<int>(type: "integer", nullable: true),
                result_winner_participant_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_game_sessions", x => x.id);
                table.ForeignKey(
                    name: "fk_game_sessions_game_session_participants_result_winner_parti",
                    column: x => x.result_winner_participant_id,
                    principalSchema: "game_runtime",
                    principalTable: "game_session_participants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_game_session_participants_session_id",
            schema: "game_runtime",
            table: "game_session_participants",
            column: "session_id");

        migrationBuilder.CreateIndex(
            name: "ix_game_sessions_result_winner_participant_id",
            schema: "game_runtime",
            table: "game_sessions",
            column: "result_winner_participant_id");

        migrationBuilder.AddForeignKey(
            name: "fk_game_session_participants_game_sessions_session_id",
            schema: "game_runtime",
            table: "game_session_participants",
            column: "session_id",
            principalSchema: "game_runtime",
            principalTable: "game_sessions",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_game_session_participants_game_sessions_session_id",
            schema: "game_runtime",
            table: "game_session_participants");

        migrationBuilder.DropTable(
            name: "game_sessions",
            schema: "game_runtime");

        migrationBuilder.DropTable(
            name: "game_session_participants",
            schema: "game_runtime");
    }
}
