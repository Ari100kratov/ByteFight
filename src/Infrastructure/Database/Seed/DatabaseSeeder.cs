namespace Infrastructure.Database.Seed;

public class DatabaseSeeder(UserDataSeeder authSeeder, GameDataSeeder gameSeeder, AuthorizationDataSeeder authorizationDataSeeder)
{
    public async Task Seed(CancellationToken cancellationToken = default)
    {
        var seed = new SeedContext();

        await authorizationDataSeeder.Seed(cancellationToken);
        await authSeeder.Seed(seed, cancellationToken);
        await gameSeeder.Seed(seed, cancellationToken);
    }
}
