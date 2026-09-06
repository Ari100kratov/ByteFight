using Application.Abstractions.Data;
using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.Characters;
using Domain.GameRuntime.GameActionLogs;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.ArenaItems;
using GameRuntime.Common.World.Stats;
using GameRuntime.Common.World.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace GameRuntime.Builders;

/// <summary>
/// Собирает боевой мир из данных арены, персонажа и его талантов.
/// </summary>
internal sealed class ArenaWorldBuilder(IServiceScopeFactory scopeFactory)
{
    public async Task<Result<ArenaWorld>> Build(Guid arenaId, Guid characterId, CancellationToken ct)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IGameDbContext dbContext = scope.ServiceProvider.GetRequiredService<IGameDbContext>();

        Arena? arena = await dbContext.Arenas
            .AsNoTracking()
            .Where(a => a.Id == arenaId)
            .Include(a => a.Items)
                .ThenInclude(x => x.Item)
            .Include(a => a.TerrainCells)
            .SingleOrDefaultAsync(ct);

        if (arena is null)
        {
            return Result.Failure<ArenaWorld>(ArenaErrors.NotFound(arenaId));
        }

        Character? character = await dbContext.Characters
            .AsNoTracking()
            .Where(x => x.Id == characterId)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Stats)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Abilities)
            .Include(x => x.Talents)
            .SingleOrDefaultAsync(ct);

        if (character is null)
        {
            return Result.Failure<ArenaWorld>(CharacterErrors.NotFound(characterId));
        }

        List<ArenaEnemy> arenaEnemies = await dbContext.ArenaEnemies
            .AsNoTracking()
            .Where(e => e.ArenaId == arena.Id)
            .OrderByDescending(e => e.Position.Y)
            .ThenByDescending(e => e.Position.X)
            .Include(e => e.Enemy)
                .ThenInclude(e => e.Stats)
            .Include(e => e.Enemy)
                .ThenInclude(e => e.Abilities)
            .ToListAsync(ct);

        // Команды боя: игрок — одна, враги арены — другая.
        var playerTeam = Guid.NewGuid();
        var enemyTeam = Guid.NewGuid();

        PlayerUnit player = BuildPlayerUnit(character, arena, playerTeam);
        TalentEffectsApplier.Apply(player, character.GetTalentRanks());

        var enemyUnits = new List<EnemyUnit>(arenaEnemies.Count);

        foreach (ArenaEnemy arenaEnemy in arenaEnemies)
        {
            var enemyUnit = new EnemyUnit(arenaEnemy.Position, FacingDirection.Left)
            {
                Name = arenaEnemy.Enemy.Name,
                ArenaEnemyId = arenaEnemy.Id,
                EnemyId = arenaEnemy.EnemyId,
                TeamId = enemyTeam,
                Stats = new RuntimeStats(arenaEnemy.Enemy.Stats.Select(x => (x.StatType, x.Value))),
                Abilities = BuildEnemyAbilities(arenaEnemy.Enemy.Abilities)
            };

            enemyUnits.Add(enemyUnit);
        }

        var arenaWorld = new ArenaWorld
        {
            Arena = new ArenaDefinition
            {
                ArenaId = arena.Id,
                GridWidth = arena.GridWidth,
                GridHeight = arena.GridHeight,
                StartPosition = arena.StartPosition,
                BlockedPositions = [.. arena.BlockedPositions],
                Terrain = arena.TerrainCells.ToDictionary(
                    x => x.Position,
                    x => x.Terrain),
                Items =
                [
                    .. arena.Items.Select(x => new ArenaItemDefinition
                    {
                        PlacedItemId = x.Id,
                        ItemId = x.ItemId,
                        Name = x.Item.Name,
                        Type = x.Item.Type,
                        Position = x.Position,
                        Value = x.Item.Value,
                    })
                ]
            },

            Player = player,

            Enemies = enemyUnits
        };

        return arenaWorld;
    }

    /// <summary>
    /// Создаёт аватар игрока: базовые характеристики спека
    /// и способности по каталогу.
    /// </summary>
    private static PlayerUnit BuildPlayerUnit(Character character, Arena arena, Guid teamId)
    {
        IReadOnlyCollection<Domain.Game.CharacterSpecs.CharacterSpecStat> specStats =
            character.Spec.Stats ?? [];
        IReadOnlyCollection<Domain.Game.CharacterSpecAbilities.CharacterSpecAbility> specAbilities =
            character.Spec.Abilities ?? [];

        return new PlayerUnit(arena.StartPosition, FacingDirection.Right)
        {
            Name = character.Name,
            CharacterId = character.Id,
            Spec = character.Spec.Type,
            Level = character.Level,
            TeamId = teamId,
            Stats = new RuntimeStats(specStats.Select(x => (x.StatType, x.Value))),
            Abilities = BuildAbilities(specAbilities.Select(x => (x.Type, x.Priority)))
        };
    }

    private static RuntimeAbilities BuildAbilities(
        IEnumerable<(Domain.Game.Abilities.AbilityType Type, int Priority)> abilities) =>
        new(abilities.Select(x => RuntimeAbilityFactory.Create(x.Type, x.Priority)));

    private static RuntimeAbilities BuildEnemyAbilities(
        IEnumerable<Domain.Game.Enemies.EnemyAbility> abilities) =>
        new(abilities.Select(x => RuntimeAbilityFactory.Create(x.Type, x.Priority)));
}
