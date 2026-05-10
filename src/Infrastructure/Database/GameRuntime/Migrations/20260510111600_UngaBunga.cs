using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
public partial class UngaBunga : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "actor_hp_current",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "numeric",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "actor_hp_max",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "numeric",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "item_id",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "item_name",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "item_picked_up_log_entry_value",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "numeric",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "item_type",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "placed_item_id",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "position_x",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "position_y",
            schema: "game_runtime",
            table: "game_action_log_entries",
            type: "integer",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "actor_hp_current",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "actor_hp_max",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "item_id",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "item_name",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "item_picked_up_log_entry_value",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "item_type",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "placed_item_id",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "position_x",
            schema: "game_runtime",
            table: "game_action_log_entries");

        migrationBuilder.DropColumn(
            name: "position_y",
            schema: "game_runtime",
            table: "game_action_log_entries");
    }
}
