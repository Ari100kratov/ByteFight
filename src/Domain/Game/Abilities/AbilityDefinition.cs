using Domain.ValueObjects;

namespace Domain.Game.Abilities;

/// <summary>
/// Полное определение способности: игровой источник правды.
/// Значения используются и сидированием БД, и боевым рантаймом,
/// поэтому баланс меняется в одном месте.
/// </summary>
/// <param name="Type">Тип способности.</param>
/// <param name="Name">Название для игрока.</param>
/// <param name="Description">Описание для игрока.</param>
/// <param name="EffectType">Тип эффекта.</param>
/// <param name="TargetType">Тип цели.</param>
/// <param name="Shape">Форма области на гекс-поле.</param>
/// <param name="Stats">Характеристики: дальность, урон, лечение, стоимость и перезарядка.</param>
/// <param name="Statuses">Статус-эффекты, накладываемые на поражённые цели.</param>
/// <param name="DashesToTarget">Рывок применяющего к цели.</param>
public sealed record AbilityDefinition(
    AbilityType Type,
    string Name,
    string Description,
    AbilityEffectType EffectType,
    AbilityTargetType TargetType,
    AbilityShape Shape,
    IReadOnlyDictionary<AbilityStatType, decimal> Stats,
    IReadOnlyList<AbilityStatusEffect>? Statuses = null,
    bool DashesToTarget = false)
{
    /// <summary>
    /// Возвращает значение характеристики способности.
    /// </summary>
    public decimal Get(AbilityStatType stat) => Stats.GetValueOrDefault(stat);
}
