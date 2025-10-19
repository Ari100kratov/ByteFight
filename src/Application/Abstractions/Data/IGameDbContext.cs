using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.Characters;
using Domain.Game.Characters.CharacterCodes;
using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IGameDbContext
{
    DbSet<Character> Characters { get; }
    DbSet<CharacterCode> CharacterCodes { get; }

    DbSet<Arena> Arenas { get; }
    DbSet<ArenaEnemy> ArenaEnemies { get; }

    DbSet<Enemy> Enemies { get; }
    DbSet<EnemyAsset> EnemyAssets { get; }
    DbSet<EnemyStat> EnemyStats { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
