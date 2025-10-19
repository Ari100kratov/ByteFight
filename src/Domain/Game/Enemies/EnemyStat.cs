using Domain.Game.Stats;

namespace Domain.Game.Enemies;

public class EnemyStat
{
    public Guid EnemyId { get; set; }
    public StatType StatType { get; set; }
    public decimal Value { get; set; }

    public Enemy Enemy { get; set; }
}
