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
        IReadOnlyList<ArenaItemDefinition>? items = null,
        IReadOnlyDictionary<Position, Domain.Game.Arenas.TerrainType>? terrain = null) =>
        new()
        {
            Arena = new ArenaDefinition
            {
                ArenaId = Guid.CreateVersion7(),
                GridWidth = gridWidth,
                GridHeight = gridHeight,
                StartPosition = new Position(0, 0),
                BlockedPositions = blockedPositions?.ToArray() ?? [],
                Terrain = terrain ?? new Dictionary<Position, Domain.Game.Arenas.TerrainType>(),
                Items = items ?? []
            },
            Player = player,
            Enemies = enemies
        };

    public static PlayerUnit CreatePlayer(
        Position position,
        decimal health = 20,
        decimal moveRange = 2,
        decimal initiative = 5,
        RuntimeAbilities? abilities = null) =>
        new(position, FacingDirection.Right)
        {
            CharacterId = Guid.CreateVersion7(),
            Name = "Игрок",
            Spec = CharacterSpecType.Berserker,
            Stats = CreateStats(health, moveRange, initiative),
            Abilities = abilities ?? new RuntimeAbilities([])
        };

    public static EnemyUnit CreateEnemy(
        Position position,
        decimal health = 20,
        decimal moveRange = 2,
        decimal initiative = 3,
        RuntimeAbilities? abilities = null) =>
        new(position, FacingDirection.Left)
        {
            ArenaEnemyId = Guid.CreateVersion7(),
            EnemyId = Guid.CreateVersion7(),
            Name = "Враг",
            Stats = CreateStats(health, moveRange, initiative),
            Abilities = abilities ?? new RuntimeAbilities([])
        };

    private static RuntimeStats CreateStats(decimal health, decimal moveRange, decimal initiative) =>
        new([
            (StatType.Health, health),
            (StatType.MoveRange, moveRange),
            (StatType.Initiative, initiative)
        ]);
}
