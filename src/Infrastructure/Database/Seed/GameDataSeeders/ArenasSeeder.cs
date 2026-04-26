using Application.Abstractions.Data;
using Domain;
using Domain.Game.Arenas;
using Domain.Game.GameModes;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal class ArenasSeeder
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        var trainingArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Тихая поляна",
            Description = "Небольшая лесная поляна, скрытая среди густых деревьев и зарослей кустарника." +
                "Тишину нарушают лишь тяжёлые шаги двух зелёных орков, вышедших из чащи...",
            BackgroundAsset = "arenas/quiet-clearing/background.png",
            ImageUrl = "arenas/quiet-clearing/preview.png",
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

        var skeletonCryptArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Проклятая крипта",
            Description = "Мрачная каменная арена среди костей, могил и огненных факелов. " +
                "Из глубины проклятой крипты поднимаются скелеты, готовые защищать это место от чужаков...",
            BackgroundAsset = "arenas/cursed-crypt/background.png",
            ImageUrl = "arenas/cursed-crypt/preview.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(seed.AdminId),
            CreatedAt = DateTime.UtcNow
        };

        skeletonCryptArena.SetSize(10, 10);
        skeletonCryptArena.SetStartPosition(new Position(4, 0));
        skeletonCryptArena.SetBlockedPositions([
            new(0, 0), new(1, 0), new(2, 0), new(6, 0), new(7, 0), new(8, 0), new(9, 0),
            new(0, 1), new(1, 1), new(2, 1), new(7, 1), new(8, 1), new(9, 1),
            new(0, 2), new(1, 2), new(2, 2), new(2, 2), new(7, 2), new(8, 2), new(9, 2),
            new(0, 3), new(2, 3), new(8, 3),
            new(3, 4),
            new(1, 5),
            new(0, 6), new(7, 6), new(9, 6),
            new(0, 7), new(8, 7), new(9, 7),
            new(0, 8), new(1, 8), new(2, 8), new(8, 8), new(9, 8),
            new(0, 9), new(1, 9), new(2, 9), new(3, 9), new(4, 9), new(5, 9), new(6, 9), new(7, 9), new(8, 9), new(9, 9)
        ]);

        dbContext.Arenas.AddRange(trainingArena, skeletonCryptArena);

        seed.TrainingArena = trainingArena.Id;
        seed.SkeletonCryptArena = skeletonCryptArena.Id;
    }
}
