using Domain.Game.Abilities;
using Domain.Game.Statuses;
using Domain.ValueObjects;

namespace GameRuntime.Common.World.Abilities;

/// <summary>
/// Описывает способность юнита, доступную во время боя.
/// </summary>
public sealed class RuntimeAbility
{
    /// <summary>
    /// Тип способности.
    /// </summary>
    public required AbilityType Type { get; init; }

    /// <summary>
    /// Тип эффекта способности.
    /// </summary>
    public required AbilityEffectType EffectType { get; init; }

    /// <summary>
    /// Тип цели способности.
    /// </summary>
    public required AbilityTargetType TargetType { get; init; }

    /// <summary>
    /// Форма области действия на гекс-поле.
    /// </summary>
    public AbilityShape Shape { get; init; } = AbilityShape.SingleTarget;

    /// <summary>
    /// Название способности.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Приоритет выбора способности.
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Характеристики способности. Изменяются талантами при старте боя.
    /// </summary>
    public IReadOnlyDictionary<AbilityStatType, decimal> Stats { get; set; } =
        new Dictionary<AbilityStatType, decimal>();

    /// <summary>
    /// Статус-эффекты, накладываемые способностью на поражённые цели.
    /// Изменяются талантами при старте боя.
    /// </summary>
    public IReadOnlyList<AbilityStatusEffect> AppliedStatuses { get; set; } = [];

    /// <summary>
    /// Способность перемещает применяющего к цели (богатырский рывок).
    /// </summary>
    public bool DashesToTarget { get; init; }

    /// <summary>
    /// Возвращает значение характеристики способности.
    /// </summary>
    public decimal Get(AbilityStatType statType) =>
        Stats.GetValueOrDefault(statType);

    /// <summary>
    /// Проверяет, есть ли у способности указанная характеристика.
    /// </summary>
    public bool Has(AbilityStatType statType) =>
        Stats.ContainsKey(statType);

    /// <summary>
    /// Возвращает дальность применения способности.
    /// </summary>
    public int GetRange() =>
        decimal.ToInt32(Math.Ceiling(Get(AbilityStatType.Range)));

    /// <summary>
    /// Возвращает перезарядку способности в ходах.
    /// </summary>
    public int GetCooldown() =>
        decimal.ToInt32(Get(AbilityStatType.Cooldown));

    /// <summary>
    /// Возвращает стоимость в очках действия.
    /// </summary>
    public int GetActionCost() =>
        Stats.TryGetValue(AbilityStatType.ActionCost, out decimal cost) && cost > 0
            ? decimal.ToInt32(cost)
            : 1;

    /// <summary>
    /// Возвращает стоимость в мане.
    /// </summary>
    public decimal GetManaCost() =>
        Get(AbilityStatType.ManaCost);

    /// <summary>
    /// Возвращает радиус области действия.
    /// </summary>
    public int GetAreaRadius() =>
        decimal.ToInt32(Get(AbilityStatType.AreaRadius));

    /// <summary>
    /// Проверяет, может ли способность быть применена на указанной дистанции.
    /// </summary>
    public bool CanReach(int distance) =>
        distance <= GetRange();

    /// <summary>
    /// Является ли способность базовой атакой.
    /// </summary>
    public bool IsBasicAttack =>
        Type is AbilityType.BasicMeleeAttack or AbilityType.BasicRangedAttack;

    /// <summary>
    /// Добавляет плоскую прибавку к характеристике способности
    /// (используется талантами).
    /// </summary>
    public void AddStat(AbilityStatType statType, decimal delta)
    {
        var mutable = new Dictionary<AbilityStatType, decimal>(Stats);
        mutable[statType] = mutable.GetValueOrDefault(statType) + delta;
        Stats = mutable;
    }

    /// <summary>
    /// Усиливает конкретный статус-эффект способности.
    /// </summary>
    public void BoostStatusMagnitude(StatusEffectType status, decimal delta)
    {
        List<AbilityStatusEffect> updated = [];

        foreach (AbilityStatusEffect effect in AppliedStatuses)
        {
            updated.Add(effect.Type == status
                ? effect with { Magnitude = effect.Magnitude + delta }
                : effect);
        }

        AppliedStatuses = updated;
    }

    /// <summary>
    /// Продлевает все статус-эффекты способности.
    /// </summary>
    public void BoostStatusDuration(int extraTurns)
    {
        List<AbilityStatusEffect> updated = [];

        foreach (AbilityStatusEffect effect in AppliedStatuses)
        {
            updated.Add(effect with { Duration = effect.Duration + extraTurns });
        }

        AppliedStatuses = updated;
    }
}
