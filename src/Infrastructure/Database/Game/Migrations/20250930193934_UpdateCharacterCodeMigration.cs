using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCharacterCodeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_character_code_characters_character_id",
                schema: "game",
                table: "character_code");

            migrationBuilder.DropPrimaryKey(
                name: "pk_character_code",
                schema: "game",
                table: "character_code");

            migrationBuilder.RenameTable(
                name: "character_code",
                schema: "game",
                newName: "character_codes",
                newSchema: "game");

            migrationBuilder.RenameIndex(
                name: "ix_character_code_character_id",
                schema: "game",
                table: "character_codes",
                newName: "ix_character_codes_character_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_character_codes",
                schema: "game",
                table: "character_codes",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_character_codes_characters_character_id",
                schema: "game",
                table: "character_codes",
                column: "character_id",
                principalSchema: "game",
                principalTable: "characters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_character_codes_characters_character_id",
                schema: "game",
                table: "character_codes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_character_codes",
                schema: "game",
                table: "character_codes");

            migrationBuilder.RenameTable(
                name: "character_codes",
                schema: "game",
                newName: "character_code",
                newSchema: "game");

            migrationBuilder.RenameIndex(
                name: "ix_character_codes_character_id",
                schema: "game",
                table: "character_code",
                newName: "ix_character_code_character_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_character_code",
                schema: "game",
                table: "character_code",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_character_code_characters_character_id",
                schema: "game",
                table: "character_code",
                column: "character_id",
                principalSchema: "game",
                principalTable: "characters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
