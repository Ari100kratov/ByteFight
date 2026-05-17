using Domain.Game.CharacterSpecs;
using Domain.Game.Stats;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.ArenaItems;
using GameRuntime.Common.World.Stats;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Common.UnitTests;

internal static class TestWorldFactory
{
    public static ArenaWorld CreateWorld(
        PlayerUnit player,
        IReadOnlyList<EnemyUnit> enemies,
        int gridWidth = 3,
        int gridHeight = 3,
        IReadOnlyList<Position>? blockedPositions = null,
        IReadOnlyList<ArenaItemDefinition>? items = null) =>
        new()
        {
            Arena = new ArenaDefinition
            {
                ArenaId = Guid.CreateVersion7(),
                GridWidth = gridWidth,
                GridHeight = gridHeight,
                StartPosition = new Position(0, 0),
                BlockedPositions = blockedPositions?.ToArray() ?? [],
                Items = items ?? []
            },
            Player = player,
            Enemies = enemies
        };

    public static PlayerUnit CreatePlayer(Position position, decimal health = 20, decimal moveRange = 2) =>
        new(position, FacingDirection.Right)
        {
            CharacterId = Guid.CreateVersion7(),
            Name = "Player",
            Spec = CharacterSpecType.Berserker,
            Stats = CreateStats(health, moveRange),
            Abilities = new RuntimeAbilities([])
        };

    public static EnemyUnit CreateEnemy(Position position, decimal health = 20, decimal moveRange = 2) =>
        new(position, FacingDirection.Left)
        {
            ArenaEnemyId = Guid.CreateVersion7(),
            EnemyId = Guid.CreateVersion7(),
            Name = "Enemy",
            Stats = CreateStats(health, moveRange),
            Abilities = new RuntimeAbilities([])
        };

    private static RuntimeStats CreateStats(decimal health, decimal moveRange) =>
        new([
            (StatType.Health, health),
            (StatType.MoveRange, moveRange)
        ]);
}
