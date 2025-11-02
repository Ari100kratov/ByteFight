using Application.Abstractions.Data;
using Domain.Game.Arenas.ArenaEnemies;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal class ArenaEnemies
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        var arenaEnemies = new List<ArenaEnemy>
        {
            new()
            {
                Id = Guid.CreateVersion7(),
                ArenaId = seed.Arena_1,
                EnemyId = seed.Orc_Warrior,
                Position = new Position(2, 5)
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ArenaId = seed.Arena_1,
                EnemyId = seed.Orc_Warrior,
                Position = new Position(6, 6)
            }
        };

        dbContext.ArenaEnemies.AddRange(arenaEnemies);
    }
}
