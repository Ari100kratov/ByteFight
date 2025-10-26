using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "game");

        migrationBuilder.CreateTable(
            name: "arenas",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                grid_width = table.Column<int>(type: "integer", nullable: false),
                grid_height = table.Column<int>(type: "integer", nullable: false),
                background_asset = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                game_modes = table.Column<int[]>(type: "integer[]", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_arenas", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "character_classes",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_classes", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "enemies",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemies", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "character_class_action_assets",
            schema: "game",
            columns: table => new
            {
                character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                table.PrimaryKey("pk_character_class_action_assets", x => new { x.character_class_id, x.action_type, x.variant });
                table.ForeignKey(
                    name: "fk_character_class_action_assets_character_classes_character_c",
                    column: x => x.character_class_id,
                    principalSchema: "game",
                    principalTable: "character_classes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "character_class_stats",
            schema: "game",
            columns: table => new
            {
                character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                stat_type = table.Column<int>(type: "integer", nullable: false),
                value = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_class_stats", x => new { x.character_class_id, x.stat_type });
                table.ForeignKey(
                    name: "fk_character_class_stats_character_classes_character_class_id",
                    column: x => x.character_class_id,
                    principalSchema: "game",
                    principalTable: "character_classes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "characters",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                class_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_characters", x => x.id);
                table.ForeignKey(
                    name: "fk_characters_character_classes_class_id",
                    column: x => x.class_id,
                    principalSchema: "game",
                    principalTable: "character_classes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "arena_enemies",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                arena_id = table.Column<Guid>(type: "uuid", nullable: false),
                enemy_id = table.Column<Guid>(type: "uuid", nullable: false),
                position_x = table.Column<int>(type: "integer", nullable: false),
                position_y = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_arena_enemies", x => x.id);
                table.ForeignKey(
                    name: "fk_arena_enemies_arenas_arena_id",
                    column: x => x.arena_id,
                    principalSchema: "game",
                    principalTable: "arenas",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_arena_enemies_enemies_enemy_id",
                    column: x => x.enemy_id,
                    principalSchema: "game",
                    principalTable: "enemies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "enemy_action_assets",
            schema: "game",
            columns: table => new
            {
                enemy_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                table.PrimaryKey("pk_enemy_action_assets", x => new { x.enemy_id, x.action_type, x.variant });
                table.ForeignKey(
                    name: "fk_enemy_action_assets_enemies_enemy_id",
                    column: x => x.enemy_id,
                    principalSchema: "game",
                    principalTable: "enemies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "enemy_stats",
            schema: "game",
            columns: table => new
            {
                enemy_id = table.Column<Guid>(type: "uuid", nullable: false),
                stat_type = table.Column<int>(type: "integer", nullable: false),
                value = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_enemy_stats", x => new { x.enemy_id, x.stat_type });
                table.ForeignKey(
                    name: "fk_enemy_stats_enemies_enemy_id",
                    column: x => x.enemy_id,
                    principalSchema: "game",
                    principalTable: "enemies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "character_codes",
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
                table.PrimaryKey("pk_character_codes", x => x.id);
                table.ForeignKey(
                    name: "fk_character_codes_characters_character_id",
                    column: x => x.character_id,
                    principalSchema: "game",
                    principalTable: "characters",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_arena_enemies_arena_id",
            schema: "game",
            table: "arena_enemies",
            column: "arena_id");

        migrationBuilder.CreateIndex(
            name: "ix_arena_enemies_enemy_id",
            schema: "game",
            table: "arena_enemies",
            column: "enemy_id");

        migrationBuilder.CreateIndex(
            name: "ix_arenas_name",
            schema: "game",
            table: "arenas",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_character_classes_name",
            schema: "game",
            table: "character_classes",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_character_codes_character_id",
            schema: "game",
            table: "character_codes",
            column: "character_id");

        migrationBuilder.CreateIndex(
            name: "ix_characters_class_id",
            schema: "game",
            table: "characters",
            column: "class_id");

        migrationBuilder.CreateIndex(
            name: "ix_characters_name",
            schema: "game",
            table: "characters",
            column: "name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "arena_enemies",
            schema: "game");

        migrationBuilder.DropTable(
            name: "character_class_action_assets",
            schema: "game");

        migrationBuilder.DropTable(
            name: "character_class_stats",
            schema: "game");

        migrationBuilder.DropTable(
            name: "character_codes",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemy_action_assets",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemy_stats",
            schema: "game");

        migrationBuilder.DropTable(
            name: "arenas",
            schema: "game");

        migrationBuilder.DropTable(
            name: "characters",
            schema: "game");

        migrationBuilder.DropTable(
            name: "enemies",
            schema: "game");

        migrationBuilder.DropTable(
            name: "character_classes",
            schema: "game");
    }
}
