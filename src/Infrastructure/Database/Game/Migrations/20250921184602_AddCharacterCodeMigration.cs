using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class AddCharacterCodeMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "updated_at",
            schema: "game",
            table: "characters",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "character_code",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                language = table.Column<int>(type: "integer", nullable: false),
                source_code = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                character_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_code", x => x.id);
                table.ForeignKey(
                    name: "fk_character_code_characters_character_id",
                    column: x => x.character_id,
                    principalSchema: "game",
                    principalTable: "characters",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_character_code_character_id",
            schema: "game",
            table: "character_code",
            column: "character_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "character_code",
            schema: "game");

        migrationBuilder.DropColumn(
            name: "updated_at",
            schema: "game",
            table: "characters");
    }
}
