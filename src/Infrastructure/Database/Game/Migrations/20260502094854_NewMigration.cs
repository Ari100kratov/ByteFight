using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class NewMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "character_spec_abilities",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                character_spec_id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                effect_type = table.Column<int>(type: "integer", nullable: false),
                target_type = table.Column<int>(type: "integer", nullable: false),
                name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                priority = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_spec_abilities", x => x.id);
                table.ForeignKey(
                    name: "fk_character_spec_abilities_character_specs_character_spec_id",
                    column: x => x.character_spec_id,
                    principalSchema: "game",
                    principalTable: "character_specs",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "enemy_abilities",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                enemy_id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                effect_type = table.Column<int>(type: "integer", nullable: false),
                target_type = table.Column<int>(type: "integer", nullable: false),
                name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                priority = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemy_abilities", x => x.id);
                table.ForeignKey(
                    name: "fk_enemy_abilities_enemies_enemy_id",
                    column: x => x.enemy_id,
                    principalSchema: "game",
                    principalTable: "enemies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "character_spec_ability_action_assets",
            schema: "game",
            columns: table => new
            {
                character_spec_ability_id = table.Column<Guid>(type: "uuid", nullable: false),
                action_type = table.Column<int>(type: "integer", nullable: false),
                variant = table.Column<int>(type: "integer", nullable: false),
                animation_url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                animation_frame_count = table.Column<int>(type: "integer", nullable: false),
                animation_animation_speed = table.Column<float>(type: "real", nullable: false),
                animation_scale_x = table.Column<float>(type: "real", nullable: false),
                animation_scale_y = table.Column<float>(type: "real", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_spec_ability_action_assets", x => new { x.character_spec_ability_id, x.action_type, x.variant });
                table.ForeignKey(
                    name: "fk_character_spec_ability_action_assets_character_spec_abiliti",
                    column: x => x.character_spec_ability_id,
                    principalSchema: "game",
                    principalTable: "character_spec_abilities",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "character_spec_ability_stats",
            schema: "game",
            columns: table => new
            {
                character_spec_ability_id = table.Column<Guid>(type: "uuid", nullable: false),
                stat_type = table.Column<int>(type: "integer", nullable: false),
                value = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_spec_ability_stats", x => new { x.character_spec_ability_id, x.stat_type });
                table.ForeignKey(
                    name: "fk_character_spec_ability_stats_character_spec_abilities_chara",
                    column: x => x.character_spec_ability_id,
                    principalSchema: "game",
                    principalTable: "character_spec_abilities",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "enemy_ability_action_assets",
            schema: "game",
            columns: table => new
            {
                enemy_ability_id = table.Column<Guid>(type: "uuid", nullable: false),
                action_type = table.Column<int>(type: "integer", nullable: false),
                variant = table.Column<int>(type: "integer", nullable: false),
                animation_url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                animation_frame_count = table.Column<int>(type: "integer", nullable: false),
                animation_animation_speed = table.Column<float>(type: "real", nullable: false),
                animation_scale_x = table.Column<float>(type: "real", nullable: false),
                animation_scale_y = table.Column<float>(type: "real", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemy_ability_action_assets", x => new { x.enemy_ability_id, x.action_type, x.variant });
                table.ForeignKey(
                    name: "fk_enemy_ability_action_assets_enemy_abilities_enemy_ability_id",
                    column: x => x.enemy_ability_id,
                    principalSchema: "game",
                    principalTable: "enemy_abilities",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "enemy_ability_stats",
            schema: "game",
            columns: table => new
            {
                enemy_ability_id = table.Column<Guid>(type: "uuid", nullable: false),
                stat_type = table.Column<int>(type: "integer", nullable: false),
                value = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemy_ability_stats", x => new { x.enemy_ability_id, x.stat_type });
                table.ForeignKey(
                    name: "fk_enemy_ability_stats_enemy_abilities_enemy_ability_id",
                    column: x => x.enemy_ability_id,
                    principalSchema: "game",
                    principalTable: "enemy_abilities",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_character_spec_abilities_character_spec_id_type",
            schema: "game",
            table: "character_spec_abilities",
            columns: ["character_spec_id", "type"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_enemy_abilities_enemy_id_type",
            schema: "game",
            table: "enemy_abilities",
            columns: ["enemy_id", "type"],
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "character_spec_ability_action_assets",
            schema: "game");

        migrationBuilder.DropTable(
            name: "character_spec_ability_stats",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemy_ability_action_assets",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemy_ability_stats",
            schema: "game");

        migrationBuilder.DropTable(
            name: "character_spec_abilities",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemy_abilities",
            schema: "game");
    }
}
