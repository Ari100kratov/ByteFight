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
            ..CreateQuietClearingArenaEnemies(seed),
            ..CreateSkeletonCryptArenaEnemies(seed),
            ..CreateOrcRitualArenaEnemies(seed)
        ];

        dbContext.ArenaEnemies.AddRange(arenaEnemies);
    }

    private static ArenaEnemy[] CreateQuietClearingArenaEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.Quiet_Clearing_Arena, seed.Orc_Warrior, new Position(5, 3)),
        CreateArenaEnemy(seed.Quiet_Clearing_Arena, seed.Orc_Warrior, new Position(6, 5))
    ];

    private static ArenaEnemy[] CreateSkeletonCryptArenaEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.Skeleton_Crypt_Arena, seed.Skeleton, new Position(6, 5)),
        CreateArenaEnemy(seed.Skeleton_Crypt_Arena, seed.Skeleton, new Position(7, 3)),
        CreateArenaEnemy(seed.Skeleton_Crypt_Arena, seed.Skeleton, new Position(3, 3)),
        CreateArenaEnemy(seed.Skeleton_Crypt_Arena, seed.Skeleton, new Position(2, 5)),
        //CreateArenaEnemy(seed.Skeleton_Crypt_Arena, seed.Skeleton, new Position(4, 7)),
    ];

    private static ArenaEnemy[] CreateOrcRitualArenaEnemies(SeedContext seed) =>
    [
        CreateArenaEnemy(seed.Orc_Ritual_Arena, seed.Orc_Berserker, new Position(2, 5)),
        CreateArenaEnemy(seed.Orc_Ritual_Arena, seed.Orc_Berserker, new Position(9, 3)),
        CreateArenaEnemy(seed.Orc_Ritual_Arena, seed.Orc_Shaman, new Position(8, 6))
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
