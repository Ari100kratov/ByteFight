using Application.Abstractions.Data;
using Infrastructure.Database.Seed.GameDataSeeders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seed;

public class GameDataSeeder(IGameDbContext dbContext)
{
    public async Task Seed(SeedContext seed, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Arenas.AnyAsync(cancellationToken))
        {
            return;
        }

        ArenasSeeder.Seed(seed, dbContext);

        EnemiesSeeder.Seed(seed, dbContext);
        ArenaEnemiesSeeder.Seed(seed, dbContext);

        CharacterClassesSeeder.Seed(seed, dbContext);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
