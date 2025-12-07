using Domain.Game.Enemies;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Game.Arenas.ArenaEnemies;

public sealed class ArenaEnemy : Entity
{
    public Guid Id { get; set; }
    public Guid ArenaId { get; set; }
    public Guid EnemyId { get; set; }
    public Position Position { get; set; }

    public Arena Arena { get; set; }
    public Enemy Enemy { get; set; }
}
