using Domain.Game.Stats;
using Domain.ValueObjects;

namespace GameRuntime.Common.World.Stats;

/// <summary>
/// Хранит текущие и максимальные характеристики юнита во время боя.
/// </summary>
public sealed class RuntimeStats
{
    /// <summary>
    /// Текущие значения характеристик.
    /// </summary>
    public Dictionary<StatType, decimal> Current { get; }

    /// <summary>
    /// Максимальные значения характеристик.
    /// </summary>
    public Dictionary<StatType, decimal> Maximum { get; }

    /// <summary>
    /// Создает runtime-характеристики на основе базовых характеристик юнита.
    /// </summary>
    public RuntimeStats(IEnumerable<(StatType Key, decimal Value)> baseStats)
    {
        Maximum = baseStats.ToDictionary(x => x.Key, x => x.Value);
        Current = Maximum.ToDictionary(k => k.Key, v => v.Value);
    }

    /// <summary>
    /// Возвращает текущее значение характеристики.
    /// </summary>
    public decimal Get(StatType stat) =>
        Current.GetValueOrDefault(stat);

    /// <summary>
    /// Возвращает максимальное значение характеристики.
    /// </summary>
    public decimal GetMax(StatType stat) =>
        Maximum.GetValueOrDefault(stat);

    /// <summary>
    /// Проверяет, есть ли у юнита указанная характеристика.
    /// </summary>
    public bool Has(StatType stat) =>
        Current.ContainsKey(stat);

    /// <summary>
    /// Возвращает текущее здоровье юнита.
    /// </summary>
    public decimal GetHealth() =>
        Get(StatType.Health);

    /// <summary>
    /// Возвращает максимальное здоровье юнита.
    /// </summary>
    public decimal GetMaxHealth() =>
        GetMax(StatType.Health);

    /// <summary>
    /// Возвращает процент текущего здоровья от максимального в диапазоне 0..1.
    /// </summary>
    public decimal GetHealthPercent()
    {
        decimal maxHealth = GetMaxHealth();

        if (maxHealth <= 0)
        {
            return 0;
        }

        return GetHealth() / maxHealth;
    }

    public bool IsHealthFull() => GetHealth() >= GetMaxHealth();

    /// <summary>
    /// Возвращает дальность перемещения юнита.
    /// </summary>
    public int GetMoveRange() =>
        decimal.ToInt32(Math.Floor(Get(StatType.MoveRange)));

    /// <summary>
    /// Возвращает инициативу юнита (порядок хода в раунде).
    /// </summary>
    public int GetInitiative() =>
        decimal.ToInt32(Get(StatType.Initiative));

    /// <summary>
    /// Возвращает броню юнита (плоское снижение входящего урона).
    /// </summary>
    public decimal GetArmor() =>
        Get(StatType.Armor);

    /// <summary>
    /// Возвращает мощь юнита (процентная прибавка к урону и лечению).
    /// </summary>
    public decimal GetPower() =>
        Get(StatType.Power);

    /// <summary>
    /// Возвращает восстановление маны за ход.
    /// </summary>
    public decimal GetManaRegen() =>
        Get(StatType.ManaRegen);

    /// <summary>
    /// Пытается списать ману. Возвращает false, если маны не хватает.
    /// </summary>
    public bool TrySpendMana(decimal cost)
    {
        if (Get(StatType.Mana) < cost)
        {
            return false;
        }

        Current[StatType.Mana] -= cost;
        return true;
    }

    /// <summary>
    /// Восстанавливает ману, не превышая максимум.
    /// </summary>
    public void RegenerateMana(decimal value)
    {
        if (!Current.ContainsKey(StatType.Mana))
        {
            return;
        }

        Current[StatType.Mana] = Math.Min(GetMax(StatType.Mana), Get(StatType.Mana) + value);
    }

    /// <summary>
    /// Проверяет, мертв ли юнит.
    /// </summary>
    public bool IsDead() =>
        GetHealth() <= 0;

    /// <summary>
    /// Применяет урон к здоровью юнита и возвращает новый снимок здоровья.
    /// </summary>
    public StatSnapshot ApplyDamage(decimal value)
    {
        Current[StatType.Health] = Math.Max(0, GetHealth() - value);

        return new StatSnapshot(
            GetHealth(),
            GetMaxHealth());
    }

    /// <summary>
    /// Восстанавливает здоровье юнита,
    /// не превышая максимальное значение.
    /// </summary>
    public StatApplyResult Heal(decimal value)
    {
        decimal before = GetHealth();

        Current[StatType.Health] = Math.Min(
            GetMaxHealth(),
            before + value);

        StatSnapshot snapshot = new(
            GetHealth(),
            GetMaxHealth());

        return new StatApplyResult(
            GetHealth() - before,
            snapshot);
    }

    /// <summary>
    /// Изменяет максимальное значение характеристики и корректирует текущее значение.
    /// </summary>
    public void Modify(StatType stat, decimal delta)
    {
        Maximum[stat] = GetMax(stat) + delta;
        Current[stat] = Math.Min(Get(stat), Maximum[stat]);
    }
}

/// <summary>
/// Результат применения изменения характеристики.
/// </summary>
public sealed record StatApplyResult(
    decimal AppliedValue,
    StatSnapshot Snapshot);
