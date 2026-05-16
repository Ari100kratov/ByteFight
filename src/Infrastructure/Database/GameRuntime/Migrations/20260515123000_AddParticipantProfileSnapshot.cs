using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
[DbContext(typeof(GameRuntimeDbContext))]
[Migration("20260515123000_AddParticipantProfileSnapshot")]
public partial class AddParticipantProfileSnapshot : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "user_first_name",
            schema: "game_runtime",
            table: "game_session_participants",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "user_last_name",
            schema: "game_runtime",
            table: "game_session_participants",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "character_class_name",
            schema: "game_runtime",
            table: "game_session_participants",
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "character_spec_name",
            schema: "game_runtime",
            table: "game_session_participants",
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "user_first_name",
            schema: "game_runtime",
            table: "game_session_participants");

        migrationBuilder.DropColumn(
            name: "user_last_name",
            schema: "game_runtime",
            table: "game_session_participants");

        migrationBuilder.DropColumn(
            name: "character_class_name",
            schema: "game_runtime",
            table: "game_session_participants");

        migrationBuilder.DropColumn(
            name: "character_spec_name",
            schema: "game_runtime",
            table: "game_session_participants");
    }
}
