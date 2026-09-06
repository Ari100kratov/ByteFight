using Application.Abstractions.Data;
using Domain;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal static class AdminCharactersSeeder
{
    private static readonly (Func<SeedContext, Guid> SpecId, string Name)[] Characters =
    [
        (seed => seed.Spec_Vityaz_SwordBearer, "Боривой Ратай"),
        (seed => seed.Spec_Vityaz_ShieldBearer, "Щитослав Непробиваемый"),
        (seed => seed.Spec_Vityaz_Druzhinnik, "Горислав Дружинный"),
        (seed => seed.Spec_Volkhv_StormCaller, "Громослав Гремящий"),
        (seed => seed.Spec_Volkhv_WardKeeper, "Светозар Обережный"),
        (seed => seed.Spec_Volkhv_Veden, "Ведан Мудрый"),
        (seed => seed.Spec_Okhotnik_Archer, "Лучеслав Точный"),
        (seed => seed.Spec_Okhotnik_Trappers, "Силан Ловчий"),
        (seed => seed.Spec_Okhotnik_BeastStalker, "Зверан Хожалый"),
        (seed => seed.Spec_Vedunya_Herbalist, "Травена Зелёная"),
        (seed => seed.Spec_Vedunya_Whisperer, "Шептана Тёмная"),
        (seed => seed.Spec_Vedunya_Sorceress, "Ворожена Ведающая")
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
