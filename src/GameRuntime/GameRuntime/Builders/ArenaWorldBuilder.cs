using Application.Abstractions.Data;
using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.Characters;
using Domain.Game.CharacterSpecAbilities;
using Domain.Game.Enemies;
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
            .SingleOrDefaultAsync(ct);

        if (arena is null)
        {
            return Result.Failure<ArenaWorld>(ArenaErrors.NotFound(arenaId));
        }

        Character? character = await dbContext.Characters
            .AsNoTracking()
            .Where(x => x.Id == characterId)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Stats)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Include(x => x.Spec)
                .ThenInclude(x => x.Abilities)
                    .ThenInclude(x => x.Stats)
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
                    .ThenInclude(e => e.Stats)
            .ToListAsync(ct);

        var enemyUnits = new List<EnemyUnit>(arenaEnemies.Count);

        foreach (ArenaEnemy arenaEnemy in arenaEnemies)
        {
            var enemyUnit = new EnemyUnit(arenaEnemy.Position, FacingDirection.Left)
            {
                Name = arenaEnemy.Enemy.Name,
                ArenaEnemyId = arenaEnemy.Id,
                EnemyId = arenaEnemy.EnemyId,
                Stats = new RuntimeStats(arenaEnemy.Enemy.Stats.Select(x => (x.StatType, x.Value))),
                Abilities = CreateEnemyAbilities(arenaEnemy.Enemy.Abilities)
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
                Items = [.. arena.Items.Select(x => new ArenaItemDefinition
                {
                    PlacedItemId = x.Id,
                    ItemId = x.ItemId,
                    Name = x.Item.Name,
                    Type = x.Item.Type,
                    Position = x.Position,
                    Value = x.Item.Value,
                })]
            },

            Player = new PlayerUnit(arena.StartPosition, FacingDirection.Right)
            {
                Name = character.Name,
                CharacterId = character.Id,
                Spec = character.Spec.Type,
                Stats = new RuntimeStats(character.Spec.Stats.Select(x => (x.StatType, x.Value))),
                Abilities = CreateCharacterAbilities(character.Spec.Abilities)
            },

            Enemies = enemyUnits
        };

        return arenaWorld;
    }

    private static RuntimeAbilities CreateCharacterAbilities(
        IEnumerable<CharacterSpecAbility> abilities)
    {
        RuntimeAbility[] runtimeAbilities = [.. abilities
            .Select(x => new RuntimeAbility
            {
                Type = x.Type,
                EffectType = x.EffectType,
                TargetType = x.TargetType,
                Name = x.Name,
                Priority = x.Priority,
                Stats = x.Stats.ToDictionary(s => s.StatType, s => s.Value)
            })];

        return new RuntimeAbilities(runtimeAbilities);
    }

    private static RuntimeAbilities CreateEnemyAbilities(
        IEnumerable<EnemyAbility> abilities)
    {
        RuntimeAbility[] runtimeAbilities = [.. abilities
            .Select(x => new RuntimeAbility
            {
                Type = x.Type,
                EffectType = x.EffectType,
                TargetType = x.TargetType,
                Name = x.Name,
                Priority = x.Priority,
                Stats = x.Stats.ToDictionary(s => s.StatType, s => s.Value)
            })];

        return new RuntimeAbilities(runtimeAbilities);
    }
}
