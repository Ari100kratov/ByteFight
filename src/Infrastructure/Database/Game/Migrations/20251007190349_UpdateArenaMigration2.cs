using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class UpdateArenaMigration2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "description",
            schema: "game",
            table: "arenas",
            type: "character varying(256)",
            maxLength: 256,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "description",
            schema: "game",
            table: "arenas");
    }
}
