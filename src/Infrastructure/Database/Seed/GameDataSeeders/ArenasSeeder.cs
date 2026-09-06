using Application.Abstractions.Data;
using Domain;
using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaTerrainCells;
using Domain.Game.GameModes;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

/// <summary>
/// Гексагональные арены со славянским рельефом: чаща, камни, топь.
/// </summary>
internal static class ArenasSeeder
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        Arena forestEdge = CreateForestEdge(seed.AdminId);
        Arena graveyard = CreateGraveyard(seed.AdminId);
        Arena swamp = CreateSwamp(seed.AdminId);

        dbContext.Arenas.AddRange(forestEdge, graveyard, swamp);

        seed.ForestEdge_Arena = forestEdge.Id;
        seed.Graveyard_Arena = graveyard.Id;
        seed.Swamp_Arena = swamp.Id;
    }

    /// <summary>
    /// Лесная опушка: небольшой полянный бой среди чащи (обучение).
    /// </summary>
    private static Arena CreateForestEdge(Guid adminId)
    {
        var arena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Лесная опушка",
            Description = "Светлая опушка у старого дуба. Тишина нарушена хрустом ветки: " +
                "из чащи вышли упыри, а в мху блестят чужие глаза...",
            BackgroundAsset = "arenas/forest-edge/background.png",
            ImageUrl = "arenas/forest-edge/preview.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(adminId),
            CreatedAt = DateTime.UtcNow
        };

        arena.SetSize(8, 7);
        arena.SetStartPosition(new Position(3, 0));

        // Чаща по углам и центру: дорога вдвое дороже, зато укрывает.
        arena.TerrainCells =
        [
            TerrainCell(arena, new Position(0, 1), TerrainType.Forest),
            TerrainCell(arena, new Position(1, 2), TerrainType.Forest),
            TerrainCell(arena, new Position(6, 1), TerrainType.Forest),
            TerrainCell(arena, new Position(7, 2), TerrainType.Forest),
            TerrainCell(arena, new Position(2, 4), TerrainType.Forest),
            TerrainCell(arena, new Position(5, 4), TerrainType.Forest),
            TerrainCell(arena, new Position(3, 3), TerrainType.Rock),
            TerrainCell(arena, new Position(4, 3), TerrainType.Rock),
            TerrainCell(arena, new Position(0, 5), TerrainType.Rock),
            TerrainCell(arena, new Position(7, 5), TerrainType.Rock)
        ];

        return arena;
    }

    /// <summary>
    /// Погост у храмины: камни надгробий и мёртвая вода в овраге.
    /// </summary>
    private static Arena CreateGraveyard(Guid adminId)
    {
        var arena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Погост у храмины",
            Description = "Старый погост: покосившиеся кресты, ржавые оградки " +
                "и вода в овраге, что не отражает луну. Из-под дёрна поднимаются ратники...",
            BackgroundAsset = "arenas/graveyard/background.png",
            ImageUrl = "arenas/graveyard/preview.png",
            GameModes = [GameModeType.Training, GameModeType.PvE],
            IsActive = true,
            CreatedBy = new UserId(adminId),
            CreatedAt = DateTime.UtcNow
        };

        arena.SetSize(10, 9);
        arena.SetStartPosition(new Position(4, 0));

        arena.TerrainCells =
        [
            // Надгробия: непроходимые камни.
            TerrainCell(arena, new Position(1, 2), TerrainType.Rock),
            TerrainCell(arena, new Position(3, 2), TerrainType.Rock),
            TerrainCell(arena, new Position(6, 2), TerrainType.Rock),
            TerrainCell(arena, new Position(8, 2), TerrainType.Rock),
            TerrainCell(arena, new Position(2, 5), TerrainType.Rock),
            TerrainCell(arena, new Position(7, 5), TerrainType.Rock),
            TerrainCell(arena, new Position(4, 4), TerrainType.Rock),
            TerrainCell(arena, new Position(5, 4), TerrainType.Rock),
            // Мёртвая вода в овраге.
            TerrainCell(arena, new Position(0, 7), TerrainType.Water),
            TerrainCell(arena, new Position(1, 8), TerrainType.Water),
            TerrainCell(arena, new Position(9, 7), TerrainType.Water),
            TerrainCell(arena, new Position(8, 8), TerrainType.Water),
            // Пустоши у воды: топь.
            TerrainCell(arena, new Position(2, 7), TerrainType.Swamp),
            TerrainCell(arena, new Position(7, 7), TerrainType.Swamp)
        ];

        return arena;
    }

    /// <summary>
    /// Болотная топь: большая арена с кикиморами и волколаком.
    /// </summary>
    private static Arena CreateSwamp(Guid adminId)
    {
        var arena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Болотная топь",
            Description = "Серая топь до горизонта: кочки, ржавая вода и туман, " +
                "который кажется живым. Здесь хозяйничает кикимора, а туман дышит волколаком.",
            BackgroundAsset = "arenas/swamp/background.png",
            ImageUrl = "arenas/swamp/preview.png",
            GameModes = [GameModeType.PvE],
            IsActive = true,
            CreatedBy = new UserId(adminId),
            CreatedAt = DateTime.UtcNow
        };

        arena.SetSize(12, 9);
        arena.SetStartPosition(new Position(5, 0));

        var terrain = new List<ArenaTerrainCell>();

        // Окно в топи: широкие полосы болота и воды.
        foreach (int y in new[] { 3, 4 })
        {
            for (int x = 0; x < 12; x++)
            {
                if (x is 4 or 5 or 6)
                {
                    continue;
                }

                terrain.Add(TerrainCell(arena, new Position(x, y), TerrainType.Swamp));
            }
        }

        foreach ((int x, int y) in new[]
                 {
                     (0, 5), (1, 6), (2, 7), (9, 5), (10, 6), (11, 7), (5, 6), (6, 7)
                 })
        {
            terrain.Add(TerrainCell(arena, new Position(x, y), TerrainType.Water));
        }

        foreach ((int x, int y) in new[]
                 {
                     (2, 2), (9, 2), (4, 6), (7, 6)
                 })
        {
            terrain.Add(TerrainCell(arena, new Position(x, y), TerrainType.Forest));
        }

        arena.TerrainCells = terrain;

        return arena;
    }

    private static ArenaTerrainCell TerrainCell(Arena arena, Position position, TerrainType type) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ArenaId = arena.Id,
            Arena = arena,
            Position = position,
            Terrain = type
        };
}
