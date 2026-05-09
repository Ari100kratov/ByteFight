using Domain.Game.Stats;
using Domain.ValueObjects;
using SharedKernel;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Юнит.
/// </summary>
[UserCodeApi]
public sealed class UserUnitView
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Наименование юнита
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Текущая позиция на арене.
    /// </summary>
    public Position Position { get; init; }

    /// <summary>
    /// Характеристики юнита.
    /// </summary>
    public required UserStatsView Stats { get; init; }

    /// <summary>
    /// Способности юнита.
    /// </summary>
    public required UserAbilitiesView Abilities { get; init; }

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
    /// Текущее количество маны.
    /// </summary>
    public decimal Mana => Stats.Get(StatType.Mana) ?? 0;

    /// <summary>
    /// Дальность перемещения.
    /// </summary>
    public int MoveRange => decimal.ToInt32(Math.Floor(Stats.Get(StatType.MoveRange) ?? 0));

    /// <summary>
    /// Манхэттенское расстояние до другого юнита.
    /// </summary>
    public int DistanceTo(UserUnitView other) => Position.ManhattanDistance(other.Position);

    /// <summary>
    /// Манхэттенское расстояние до указанной позиции.
    /// </summary>
    public int DistanceTo(Position position) => Position.ManhattanDistance(position);

    /// <summary>
    /// Возвращает лучшую базовую атаку, которой можно ударить указанного юнита.
    /// </summary>
    public UserAbilityView? FindBestBasicAttackFor(UserUnitView target)
    {
        int distance = DistanceTo(target);
        return Abilities.FindBestBasicAttack(distance);
    }

    /// <summary>
    /// Проверяет, может ли юнит ударить указанную цель базовой атакой.
    /// </summary>
    public bool CanAttack(UserUnitView target) =>
        FindBestBasicAttackFor(target) is not null;

    /// <summary>
    /// Возвращает лучшую базовую дальнюю атаку,
    /// которой можно ударить указанного юнита.
    /// </summary>
    public UserAbilityView? FindBestBasicRangedAttackFor(UserUnitView target)
    {
        int distance = DistanceTo(target);
        return Abilities.FindBestBasicRangedAttack(distance);
    }

    /// <summary>
    /// Проверяет, может ли юнит атаковать цель
    /// базовой дальней атакой.
    /// </summary>
    public bool CanAttackRanged(UserUnitView target) =>
        FindBestBasicRangedAttackFor(target) is not null;
}
