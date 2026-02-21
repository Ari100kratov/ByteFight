using Application.Abstractions.Data;
using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.Characters;
using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.World;
using GameRuntime.World.Stats;
using GameRuntime.World.Units;
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
            .SingleOrDefaultAsync(ct);

        if (arena is null)
        {
            return Result.Failure<ArenaWorld>(ArenaErrors.NotFound(arenaId));
        }

        Character? character = await dbContext.Characters
            .AsNoTracking()
            .Where(a => a.Id == characterId)
            .Include(a => a.Class)
                .ThenInclude(c => c.Stats)
            .SingleOrDefaultAsync(ct);

        if (character is null)
        {
            return Result.Failure<ArenaWorld>(CharacterErrors.NotFound(characterId));
        }

        List<ArenaEnemy> arenaEnemies = await dbContext.ArenaEnemies
            .AsNoTracking()
            .Where(e => e.ArenaId == arena.Id)
            .Include(e => e.Enemy)
                .ThenInclude(c => c.Stats)
            .ToListAsync(ct);

        var enemyUnits = new List<EnemyUnit>(arenaEnemies.Count);

        foreach (ArenaEnemy arenaEnemy in arenaEnemies)
        {
            var enemyUnit = new EnemyUnit(arenaEnemy.Position, FacingDirection.Left)
            {
                ArenaEnemyId = arenaEnemy.Id,
                EnemyId = arenaEnemy.EnemyId,
                Stats = new RuntimeStats(arenaEnemy.Enemy.Stats.Select(x => (x.StatType, x.Value)))
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
                BlockedPositions = [.. arena.BlockedPositions]
            },

            Player = new PlayerUnit(arena.StartPosition, FacingDirection.Right)
            {
                CharacterId = character.Id,
                Class = character.Class.Type,
                Stats = new RuntimeStats(character.Class.Stats.Select(x => (x.StatType, x.Value)))
            },

            Enemies = enemyUnits
        };

        return arenaWorld;
    }
}
