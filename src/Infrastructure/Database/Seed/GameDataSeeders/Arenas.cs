using Application.Abstractions.Data;
using Domain;
using Domain.Game.Arenas;
using Domain.Game.GameModes;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal class Arenas
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        var trainingArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Тренировочная поляна",
            Description = "Спокойная зелёная поляна на окраине деревни, где новички делают первые шаги в бою.",
            GridWidth = 8,
            GridHeight = 8,
            BackgroundAsset = "arenas/training-ground.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(seed.AdminId),
            CreatedAt = DateTime.UtcNow
        };

        var banditCampArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Заброшенный лагерь бандитов",
            Description = "Разрушенные палатки и кострища напоминают о недавних боях. В воздухе витает дым и запах железа. Отличное место, чтобы проверить свои силы против более опытных противников.",
            GridWidth = 15,
            GridHeight = 15,
            BackgroundAsset = "arenas/bandit-camp.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(seed.AdminId),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Arenas.AddRange(trainingArena, banditCampArena);

        seed.Arena_1 = trainingArena.Id;
        seed.Arena_2 = banditCampArena.Id;
    }
}
