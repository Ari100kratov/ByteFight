using Application.Abstractions.Data;
using Domain;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal static class AdminCharactersSeeder
{
    private static readonly (Func<SeedContext, Guid> SpecId, string Name)[] Characters =
    [
        (seed => seed.Spec_Warrior_Berserker, "Борис Бицепс"),
        (seed => seed.Spec_Warrior_Guardian, "Щитослав Непробиваемый"),
        (seed => seed.Spec_Warrior_Duelist, "Уколий Первый"),
        (seed => seed.Spec_Mage_Pyromancer, "Жарослав Поджигайло"),
        (seed => seed.Spec_Mage_Luminary, "Светлана Лучезарная"),
        (seed => seed.Spec_Mage_Arcanist, "Аркадий Арканович")
    ];

    public static async Task Seed(
        SeedContext seed,
        IGameDbContext dbContext,
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        if (seed.AdminId == Guid.Empty)
        {
            throw new InvalidOperationException("Admin user must be seeded before admin characters.");
        }

        bool adminHasCharacters = await dbContext.Characters
            .AnyAsync(x => x.UserId == new UserId(seed.AdminId), cancellationToken);

        if (adminHasCharacters)
        {
            return;
        }

        Character[] characters = [.. Characters
            .Select(x => new Character
            {
                Id = Guid.CreateVersion7(),
                Name = x.Name,
                SpecId = x.SpecId(seed),
                CreatedAt = utcNow,
                UserId = new UserId(seed.AdminId)
            })];

        dbContext.Characters.AddRange(characters);
    }
}
