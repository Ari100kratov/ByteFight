using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
public partial class BlaBLabvla : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ability_name",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ability_name",
            schema: "game_runtime",
            table: "game_action_log_entries");
    }
}
