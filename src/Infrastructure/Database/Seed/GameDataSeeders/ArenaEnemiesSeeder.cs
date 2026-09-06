using Application.Abstractions.Data;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal static class ArenaEnemiesSeeder
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        List<ArenaEnemy> arenaEnemies =
        [
            ..CreateForestEdgeEnemies(seed),
            ..CreateGraveyardEnemies(seed),
            ..CreateSwampEnemies(seed)
        ];

        dbContext.ArenaEnemies.AddRange(arenaEnemies);
    }

    private static ArenaEnemy[] CreateForestEdgeEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.ForestEdge_Arena, seed.Ghoul, new Position(3, 5)),
        CreateArenaEnemy(seed.ForestEdge_Arena, seed.SkeletonWarrior, new Position(5, 4))
    ];

    private static ArenaEnemy[] CreateGraveyardEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.Graveyard_Arena, seed.SkeletonWarrior, new Position(2, 4)),
        CreateArenaEnemy(seed.Graveyard_Arena, seed.SkeletonWarrior, new Position(7, 3)),
        CreateArenaEnemy(seed.Graveyard_Arena, seed.Ghoul, new Position(4, 7)),
        CreateArenaEnemy(seed.Graveyard_Arena, seed.Ghoul, new Position(6, 6)),
        CreateArenaEnemy(seed.Graveyard_Arena, seed.Leshy, new Position(8, 8))
    ];

    private static ArenaEnemy[] CreateSwampEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.Swamp_Arena, seed.Kikimora, new Position(3, 6)),
        CreateArenaEnemy(seed.Swamp_Arena, seed.Kikimora, new Position(8, 6)),
        CreateArenaEnemy(seed.Swamp_Arena, seed.Volkolak, new Position(5, 8)),
        CreateArenaEnemy(seed.Swamp_Arena, seed.Leshy, new Position(10, 8))
    ];

    private static ArenaEnemy CreateArenaEnemy(
        Guid arenaId,
        Guid enemyId,
        Position position) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ArenaId = arenaId,
            EnemyId = enemyId,
            Position = position
        };
}
