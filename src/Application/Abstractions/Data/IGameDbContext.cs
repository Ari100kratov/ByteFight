using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.CharacterClasses;
using Domain.Game.Characters;
using Domain.Game.Characters.CharacterCodes;
using Domain.Game.CharacterSpecAbilities;
using Domain.Game.CharacterSpecs;
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
    DbSet<EnemyActionAsset> EnemyActionAssets { get; }
    DbSet<EnemyStat> EnemyStats { get; }
    DbSet<EnemyAbility> EnemyAbilities { get; }
    DbSet<EnemyAbilityStat> EnemyAbilityStats { get; }
    DbSet<EnemyAbilityActionAsset> EnemyAbilityActionAssets { get; }

    DbSet<CharacterClass> CharacterClasses { get; }

    DbSet<CharacterSpec> CharacterSpecs { get; }
    DbSet<CharacterSpecActionAsset> CharacterSpecActionAssets { get; }
    DbSet<CharacterSpecStat> CharacterSpecStats { get; }
    DbSet<CharacterSpecAbility> CharacterSpecAbilities { get; }
    DbSet<CharacterSpecAbilityStat> CharacterSpecAbilityStats { get; }
    DbSet<CharacterSpecAbilityActionAsset> CharacterSpecAbilityActionAssets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
