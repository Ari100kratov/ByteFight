using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

/// <inheritdoc />
public partial class BungaUnga : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "arena_items",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                type = table.Column<int>(type: "integer", nullable: false),
                sprite_url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                sprite_frame_count = table.Column<int>(type: "integer", nullable: false),
                sprite_animation_speed = table.Column<float>(type: "real", nullable: false),
                sprite_scale_x = table.Column<float>(type: "real", nullable: false),
                sprite_scale_y = table.Column<float>(type: "real", nullable: false),
                value = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_arena_items", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "arena_placed_items",
            schema: "game",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                arena_id = table.Column<Guid>(type: "uuid", nullable: false),
                item_id = table.Column<Guid>(type: "uuid", nullable: false),
                position_x = table.Column<int>(type: "integer", nullable: false),
                position_y = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_arena_placed_items", x => x.id);
                table.ForeignKey(
                    name: "fk_arena_placed_items_arena_items_item_id",
                    column: x => x.item_id,
                    principalSchema: "game",
                    principalTable: "arena_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_arena_placed_items_arenas_arena_id",
                    column: x => x.arena_id,
                    principalSchema: "game",
                    principalTable: "arenas",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_arena_items_name",
            schema: "game",
            table: "arena_items",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_arena_placed_items_arena_id_item_id",
            schema: "game",
            table: "arena_placed_items",
            columns: ["arena_id", "item_id"]);

        migrationBuilder.CreateIndex(
            name: "ix_arena_placed_items_item_id",
            schema: "game",
            table: "arena_placed_items",
            column: "item_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "arena_placed_items",
            schema: "game");

        migrationBuilder.DropTable(
            name: "arena_items",
            schema: "game");
    }
}
