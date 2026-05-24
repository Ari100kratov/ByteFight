using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Game.Migrations;

[DbContext(typeof(GameDbContext))]
[Migration("20260524104129_UpdateSeededSkeletonAndArenaData")]
public partial class UpdateSeededSkeletonAndArenaData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            INSERT INTO game.arenas_blocked_positions (arena_id, x, y)
            SELECT a.id, 1, 0
            FROM game.arenas AS a
            WHERE a.background_asset = 'arenas/orc-ritual-ground/background.png'
              AND NOT EXISTS (
                  SELECT 1
                  FROM game.arenas_blocked_positions AS bp
                  WHERE bp.arena_id = a.id
                    AND bp.x = 1
                    AND bp.y = 0
              );

            DELETE FROM game.arenas_blocked_positions AS bp
            USING game.arenas AS a
            WHERE bp.arena_id = a.id
              AND a.background_asset = 'arenas/orc-ritual-ground/background.png'
              AND bp.x = 6
              AND bp.y = 8;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 7,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url = 'enemies/skeleton/Idle.png'
              AND action_type = 1;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 7,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url = 'enemies/skeleton/Walk.png'
              AND action_type = 2;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 8,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url = 'enemies/skeleton/Run.png'
              AND action_type = 3;

            DELETE FROM game.enemy_action_assets
            WHERE animation_url = 'enemies/skeleton/Jump.png'
              AND action_type = 6;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 2,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url = 'enemies/skeleton/Hurt.png'
              AND action_type = 7;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 4,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url = 'enemies/skeleton/Dead.png'
              AND action_type = 8;

            UPDATE game.enemy_ability_action_assets
            SET animation_frame_count = 5,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url = 'enemies/skeleton/Attack_1.png'
              AND action_type = 4
              AND variant = 0;

            UPDATE game.enemy_ability_action_assets
            SET animation_frame_count = 6,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url = 'enemies/skeleton/Attack_2.png'
              AND action_type = 4
              AND variant = 1;

            UPDATE game.enemy_ability_action_assets
            SET animation_url = 'enemies/skeleton/Attack_3.png',
                animation_frame_count = 4,
                animation_scale_x = 1.0,
                animation_scale_y = 1.0
            WHERE animation_url IN ('enemies/skeleton/Special_attack.png', 'enemies/skeleton/Attack_3.png')
              AND action_type = 4
              AND variant = 2;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DELETE FROM game.arenas_blocked_positions AS bp
            USING game.arenas AS a
            WHERE bp.arena_id = a.id
              AND a.background_asset = 'arenas/orc-ritual-ground/background.png'
              AND bp.x = 1
              AND bp.y = 0;

            INSERT INTO game.arenas_blocked_positions (arena_id, x, y)
            SELECT a.id, 6, 8
            FROM game.arenas AS a
            WHERE a.background_asset = 'arenas/orc-ritual-ground/background.png'
              AND NOT EXISTS (
                  SELECT 1
                  FROM game.arenas_blocked_positions AS bp
                  WHERE bp.arena_id = a.id
                    AND bp.x = 6
                    AND bp.y = 8
              );

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 7,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url = 'enemies/skeleton/Idle.png'
              AND action_type = 1;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 8,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url = 'enemies/skeleton/Walk.png'
              AND action_type = 2;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 7,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url = 'enemies/skeleton/Run.png'
              AND action_type = 3;

            INSERT INTO game.enemy_action_assets (
                enemy_id,
                action_type,
                variant,
                animation_url,
                animation_frame_count,
                animation_animation_speed,
                animation_scale_x,
                animation_scale_y)
            SELECT idle.enemy_id,
                   6,
                   0,
                   'enemies/skeleton/Jump.png',
                   10,
                   0.1,
                   0.8,
                   0.8
            FROM game.enemy_action_assets AS idle
            WHERE idle.animation_url = 'enemies/skeleton/Idle.png'
              AND idle.action_type = 1
              AND NOT EXISTS (
                  SELECT 1
                  FROM game.enemy_action_assets AS existing
                  WHERE existing.enemy_id = idle.enemy_id
                    AND existing.action_type = 6
                    AND existing.variant = 0
              );

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 3,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url = 'enemies/skeleton/Hurt.png'
              AND action_type = 7;

            UPDATE game.enemy_action_assets
            SET animation_frame_count = 3,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url = 'enemies/skeleton/Dead.png'
              AND action_type = 8;

            UPDATE game.enemy_ability_action_assets
            SET animation_frame_count = 7,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url = 'enemies/skeleton/Attack_1.png'
              AND action_type = 4
              AND variant = 0;

            UPDATE game.enemy_ability_action_assets
            SET animation_frame_count = 4,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url = 'enemies/skeleton/Attack_2.png'
              AND action_type = 4
              AND variant = 1;

            UPDATE game.enemy_ability_action_assets
            SET animation_url = 'enemies/skeleton/Special_attack.png',
                animation_frame_count = 5,
                animation_scale_x = 0.8,
                animation_scale_y = 0.8
            WHERE animation_url IN ('enemies/skeleton/Attack_3.png', 'enemies/skeleton/Special_attack.png')
              AND action_type = 4
              AND variant = 2;
            """);
    }
}
