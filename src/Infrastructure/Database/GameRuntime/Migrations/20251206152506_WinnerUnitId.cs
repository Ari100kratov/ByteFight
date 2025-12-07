using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
public partial class WinnerUnitId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_game_sessions_game_session_participants_result_winner_parti",
            schema: "game_runtime",
            table: "game_sessions");

        migrationBuilder.DropIndex(
            name: "ix_game_sessions_result_winner_participant_id",
            schema: "game_runtime",
            table: "game_sessions");

        migrationBuilder.RenameColumn(
            name: "result_winner_participant_id",
            schema: "game_runtime",
            table: "game_sessions",
            newName: "result_winner_unit_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "result_winner_unit_id",
            schema: "game_runtime",
            table: "game_sessions",
            newName: "result_winner_participant_id");

        migrationBuilder.CreateIndex(
            name: "ix_game_sessions_result_winner_participant_id",
            schema: "game_runtime",
            table: "game_sessions",
            column: "result_winner_participant_id");

        migrationBuilder.AddForeignKey(
            name: "fk_game_sessions_game_session_participants_result_winner_parti",
            schema: "game_runtime",
            table: "game_sessions",
            column: "result_winner_participant_id",
            principalSchema: "game_runtime",
            principalTable: "game_session_participants",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }
}
