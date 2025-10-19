using SharedKernel;

namespace Domain.Game.Enemies;

public sealed class Enemy : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<EnemyStat> Stats { get; set; }
    public ICollection<EnemyAsset> Assets { get; set; }
}
