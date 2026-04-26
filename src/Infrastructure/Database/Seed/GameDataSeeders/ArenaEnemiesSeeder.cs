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
            ..CreateTrainingArenaEnemies(seed),
            ..CreateSkeletonCryptArenaEnemies(seed)
        ];

        dbContext.ArenaEnemies.AddRange(arenaEnemies);
    }

    private static ArenaEnemy[] CreateTrainingArenaEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.TrainingArena, seed.Orc_Warrior, new Position(4, 5)),
        CreateArenaEnemy(seed.TrainingArena, seed.Orc_Warrior, new Position(6, 1))
    ];

    private static ArenaEnemy[] CreateSkeletonCryptArenaEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.SkeletonCryptArena, seed.Skeleton, new Position(8, 6)),
        CreateArenaEnemy(seed.SkeletonCryptArena, seed.Skeleton, new Position(8, 4)),
        CreateArenaEnemy(seed.SkeletonCryptArena, seed.Skeleton, new Position(3, 3))
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
