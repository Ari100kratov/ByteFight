using Domain.Game.Actions;
using SharedKernel;

namespace Domain.Game.Enemies;

public sealed class EnemyAsset : Entity
{
    public Guid EnemyId { get; set; }
    public ActionType ActionType { get; set; }
    public string Url { get; set; }

    public Enemy Enemy { get; set; }
}
