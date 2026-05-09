using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
public partial class NewMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "damage",
            schema: "game_runtime",
            table: "game_action_log_entries",
            newName: "value");

        migrationBuilder.RenameColumn(
            name: "action_type",
            schema: "game_runtime",
            table: "game_action_log_entries",
            newName: "entry_type");

        migrationBuilder.AddColumn<int>(
            name: "ability_type",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "effect_type",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "integer",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ability_type",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "effect_type",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.RenameColumn(
            name: "value",
            schema: "game_runtime",
            table: "game_action_log_entries",
            newName: "damage");

        migrationBuilder.RenameColumn(
            name: "entry_type",
            schema: "game_runtime",
            table: "game_action_log_entries",
            newName: "action_type");
    }
}
