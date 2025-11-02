using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class AddScaleValueObject : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "animation_scale_y",
            schema: "game",
            table: "enemy_action_assets",
            newName: "ScaleY");

        migrationBuilder.RenameColumn(
            name: "animation_scale_x",
            schema: "game",
            table: "enemy_action_assets",
            newName: "ScaleX");

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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ScaleY",
            schema: "game",
            table: "enemy_action_assets",
            newName: "animation_scale_y");

        migrationBuilder.RenameColumn(
            name: "ScaleX",
            schema: "game",
            table: "enemy_action_assets",
            newName: "animation_scale_x");

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
}
