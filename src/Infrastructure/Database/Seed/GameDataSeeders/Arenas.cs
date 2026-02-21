using Application.Abstractions.Data;
using Domain;
using Domain.Game.Arenas;
using Domain.Game.GameModes;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal class Arenas
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        var trainingArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Шепчущая поляна",
            Description = "Небольшая лесная поляна, скрытая среди густых деревьев и зарослей кустарника." +
                "Тишину нарушают лишь тяжёлые шаги двух зелёных орков, вышедших из чащи...",
            BackgroundAsset = "arenas/training-ground.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(seed.AdminId),
            CreatedAt = DateTime.UtcNow
        };

        trainingArena.SetSize(8, 8);
        trainingArena.SetStartPosition(new Position(0, 4));
        trainingArena.SetBlockedPositions([new(0,0), new(1, 0), new(2, 0), new(6, 0), new(7, 0), new(3, 1),
            new(1, 2), new(6, 2), new(5, 4), new(2, 5), new(3, 5), new(7, 5), new(0, 6), new(1, 6), new(2, 6),
            new(7, 6), new(0, 7), new(1, 7), new(2, 7), new(3, 7), new(4, 7), new(5, 7), new(6, 7), new(7, 7),]);

        var banditCampArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Заброшенный лагерь бандитов",
            Description = "Разрушенные палатки и кострища напоминают о недавних боях. В воздухе витает дым и запах железа. Отличное место, чтобы проверить свои силы против более опытных противников.",
            BackgroundAsset = "arenas/bandit-camp.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(seed.AdminId),
            CreatedAt = DateTime.UtcNow
        };

        banditCampArena.SetSize(15, 15);
        banditCampArena.SetStartPosition(new Position(0, 0));

        dbContext.Arenas.AddRange(trainingArena, banditCampArena);

        seed.Arena_1 = trainingArena.Id;
        seed.Arena_2 = banditCampArena.Id;
    }
}
