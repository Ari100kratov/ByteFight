using Domain.Game.Actions;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Game.Enemies;

public sealed class EnemyActionAsset : Entity
{
    public Guid EnemyId { get; set; }
    public ActionType ActionType { get; set; }
    public int Variant { get; set; }
    public SpriteAnimation Animation { get; set; }

    public Enemy Enemy { get; set; }
}
