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

        Arenas.Seed(seed, dbContext);

        Enemies.Seed(seed, dbContext);
        ArenaEnemies.Seed(seed, dbContext);

        CharacterClasses.Seed(seed, dbContext);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
