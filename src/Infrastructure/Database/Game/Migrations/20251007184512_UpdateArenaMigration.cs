using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class UpdateArenaMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "pk_arena",
            schema: "game",
            table: "arena");

        migrationBuilder.RenameTable(
            name: "arena",
            schema: "game",
            newName: "arenas",
            newSchema: "game");

        migrationBuilder.AddPrimaryKey(
            name: "pk_arenas",
            schema: "game",
            table: "arenas",
            column: "id");

        migrationBuilder.CreateIndex(
            name: "ix_arenas_name",
            schema: "game",
            table: "arenas",
            column: "name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "pk_arenas",
            schema: "game",
            table: "arenas");

        migrationBuilder.DropIndex(
            name: "ix_arenas_name",
            schema: "game",
            table: "arenas");

        migrationBuilder.RenameTable(
            name: "arenas",
            schema: "game",
            newName: "arena",
            newSchema: "game");

        migrationBuilder.AddPrimaryKey(
            name: "pk_arena",
            schema: "game",
            table: "arena",
            column: "id");
    }
}
