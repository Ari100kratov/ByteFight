using Domain.Game.Statuses;

namespace GameRuntime.Common.World.Statuses;

/// <summary>
/// Активный статус-эффект на юните во время боя.
/// </summary>
/// <param name="Type">Тип статуса.</param>
/// <param name="RemainingTurns">Оставшиеся ходы (уменьшаются в начале хода носителя).</param>
/// <param name="Magnitude">Сила эффекта.</param>
public sealed record RuntimeStatusEffect(
    StatusEffectType Type,
    int RemainingTurns,
    decimal Magnitude);

/// <summary>
/// Набор статус-эффектов юнита: наложение, обновление и тик в начале хода.
/// </summary>
public sealed class UnitStatuses
{
    private readonly Dictionary<StatusEffectType, RuntimeStatusEffect> statuses = [];

    /// <summary>
    /// Все активные статусы.
    /// </summary>
    public IReadOnlyCollection<RuntimeStatusEffect> All => statuses.Values;

    /// <summary>
    /// Оглушён ли юнит (пропускает ход).
    /// </summary>
    public bool IsStunned => Has(StatusEffectType.Stun);

    /// <summary>
    /// Скован ли юнит корнями (не может перемещаться).
    /// </summary>
    public bool IsRooted => Has(StatusEffectType.Root);

    /// <summary>
    /// Снижение очков перемещения от замедления.
    /// </summary>
    public int MovePointsPenalty =>
        decimal.ToInt32(GetValue(StatusEffectType.Slow));

    /// <summary>
    /// Плоская прибавка к исходящему урону (мощь минус немощь).
    /// </summary>
    public decimal OutgoingDamageBonus =>
        GetValue(StatusEffectType.Might) - GetValue(StatusEffectType.Weaken);

    /// <summary>
    /// Процентное снижение входящего урона (оберег).
    /// </summary>
    public decimal IncomingDamageReduction =>
        GetValue(StatusEffectType.Ward);

    public bool Has(StatusEffectType type) => statuses.ContainsKey(type);

    /// <summary>
    /// Возвращает активный статус указанного типа или null.
    /// </summary>
    public RuntimeStatusEffect? Get(StatusEffectType type) =>
        statuses.GetValueOrDefault(type);

    /// <summary>
    /// Накладывает статус. Повторное наложение того же типа берёт
    /// максимальные силу и длительность.
    /// </summary>
    public RuntimeStatusEffect Apply(StatusEffectType type, int duration, decimal magnitude)
    {
        if (statuses.TryGetValue(type, out RuntimeStatusEffect? existing))
        {
            existing = existing with
            {
                RemainingTurns = Math.Max(existing.RemainingTurns, duration),
                Magnitude = Math.Max(existing.Magnitude, magnitude)
            };

            statuses[type] = existing;
            return existing;
        }

        var effect = new RuntimeStatusEffect(type, duration, magnitude);
        statuses[type] = effect;

        return effect;
    }

    /// <summary>
    /// Перезаписывает статус точными значениями без объединения.
    /// Нулевая сила снимает статус. Используется, например,
    /// для истощения пула щита.
    /// </summary>
    public void Set(StatusEffectType type, int duration, decimal magnitude)
    {
        if (magnitude <= 0 || duration <= 0)
        {
            statuses.Remove(type);
            return;
        }

        statuses[type] = new RuntimeStatusEffect(type, duration, magnitude);
    }

    /// <summary>
    /// Уменьшает длительности всех статусов на один ход
    /// и снимает истёкшие. Возвращает снятые статусы.
    /// </summary>
    public IReadOnlyList<RuntimeStatusEffect> TickAndExpire()
    {
        List<RuntimeStatusEffect> expired = [];

        foreach (RuntimeStatusEffect effect in statuses.Values.ToList())
        {
            RuntimeStatusEffect updated = effect with
            {
                RemainingTurns = effect.RemainingTurns - 1
            };

            if (updated.RemainingTurns <= 0)
            {
                statuses.Remove(effect.Type);
                expired.Add(effect);
            }
            else
            {
                statuses[effect.Type] = updated;
            }
        }

        return expired;
    }

    /// <summary>
    /// Снимает все статусы (например, после гибели юнита).
    /// </summary>
    public void Clear() => statuses.Clear();

    private decimal GetValue(StatusEffectType type) =>
        statuses.TryGetValue(type, out RuntimeStatusEffect? effect)
            ? effect.Magnitude
            : 0;
}
