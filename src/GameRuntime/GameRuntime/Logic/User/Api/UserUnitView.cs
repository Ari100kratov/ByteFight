using Domain.Game.Stats;
using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Юнит.
/// </summary>
public sealed class UserUnitView
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Текущая позиция на арене.
    /// </summary>
    public Position Position { get; init; }

    /// <summary>
    /// Характеристики юнита.
    /// </summary>
    public UserStatsView Stats { get; init; }

    /// <summary>
    /// Признак того, что юнит мёртв.
    /// </summary>
    public bool IsDead { get; init; }


    /// <summary>
    /// Признак того, что юнит жив.
    /// </summary>
    public bool IsAlive => !IsDead;

    /// <summary>
    /// Текущее количество здоровья.
    /// </summary>
    public decimal Health => Stats.Get(StatType.Health) ?? 0;

    /// <summary>
    /// Максимальное количество здоровья.
    /// </summary>
    public decimal MaxHealth => Stats.GetSnapshot(StatType.Health)?.Max ?? 0;

    /// <summary>
    /// Доля текущего здоровья от максимального в диапазоне от 0 до 1.
    /// </summary>
    public decimal HealthPercent => MaxHealth > 0 ? Health / MaxHealth : 0;

    /// <summary>
    /// Манхэттенское расстояние до другого юнита.
    /// </summary>
    public int DistanceTo(UserUnitView other) => Position.ManhattanDistance(other.Position);

    /// <summary>
    /// Манхэттенское расстояние до указанной позиции.
    /// </summary>
    public int DistanceTo(Position position) => Position.ManhattanDistance(position);

    /// <summary>
    /// Проверяет, находится ли указанный юнит в пределах
    /// текущей дальности атаки.
    /// </summary>
    public bool IsInAttackRange(UserUnitView other)
    {
        decimal attackRange = Stats.Get(StatType.AttackRange) ?? 0;
        return DistanceTo(other) <= attackRange;
    }
}
