using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class Test : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ScaleY",
            schema: "game",
            table: "character_class_action_assets",
            newName: "animation_scale_y");

        migrationBuilder.RenameColumn(
            name: "ScaleX",
            schema: "game",
            table: "character_class_action_assets",
            newName: "animation_scale_x");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "animation_scale_y",
            schema: "game",
            table: "character_class_action_assets",
            newName: "ScaleY");

        migrationBuilder.RenameColumn(
            name: "animation_scale_x",
            schema: "game",
            table: "character_class_action_assets",
            newName: "ScaleX");
    }
}
