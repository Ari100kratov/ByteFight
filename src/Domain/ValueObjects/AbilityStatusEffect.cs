using Domain.Game.Statuses;

namespace Domain.ValueObjects;

/// <summary>
/// Статус-эффект, накладываемый способностью на цель.
/// </summary>
/// <param name="Type">Тип статуса.</param>
/// <param name="Duration">Длительность в ходах цели (считается на начале хода носителя).</param>
/// <param name="Magnitude">Сила эффекта: урон/лечение за ход, размер щита, процент или абсолютное значение.</param>
public sealed record AbilityStatusEffect(
    StatusEffectType Type,
    int Duration,
    decimal Magnitude);
