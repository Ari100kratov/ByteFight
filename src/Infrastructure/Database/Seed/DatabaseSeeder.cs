namespace Infrastructure.Database.Seed;

public class DatabaseSeeder(AuthDataSeeder authSeeder, GameDataSeeder gameSeeder)
{
    public async Task Seed(CancellationToken cancellationToken = default)
    {
        var seed = new SeedContext();

        await authSeeder.Seed(seed, cancellationToken);
        await gameSeeder.Seed(seed, cancellationToken);
    }
}
