using Domain.Game.Abilities;
using GameRuntime.Common.World.Statuses;

namespace GameRuntime.Common.World.Units;

/// <summary>
/// Изменяемое состояние юнита внутри одного боя: очки действия и перемещения,
/// перезарядки способностей и активные статусы.
/// </summary>
public sealed class UnitBattleState
{
    /// <summary>
    /// Базовое число очков действия за ход.
    /// </summary>
    public const int ActionsPerTurn = 2;

    /// <summary>
    /// Оставшиеся очки перемещения в текущем ходу.
    /// </summary>
    public int MovePointsRemaining { get; set; }

    /// <summary>
    /// Оставшиеся очки действия в текущем ходу.
    /// </summary>
    public int ActionsRemaining { get; set; }

    /// <summary>
    /// Активные статус-эффекты.
    /// </summary>
    public UnitStatuses Statuses { get; } = new();

    /// <summary>
    /// Перезарядки способностей: тип → оставшиеся ходы.
    /// </summary>
    public Dictionary<AbilityType, int> Cooldowns { get; } = [];

    /// <summary>
    /// Возвращает оставшуюся перезарядку способности (0 — готова).
    /// </summary>
    public int GetCooldown(AbilityType ability) =>
        Cooldowns.GetValueOrDefault(ability);

    /// <summary>
    /// Запускает перезарядку способности.
    /// </summary>
    public void StartCooldown(AbilityType ability, int turns)
    {
        if (turns > 0)
        {
            Cooldowns[ability] = turns;
        }
    }

    /// <summary>
    /// Уменьшает все перезарядки на один ход в начале хода юнита.
    /// </summary>
    public void TickCooldowns()
    {
        foreach (AbilityType ability in Cooldowns.Keys.ToList())
        {
            int remaining = Cooldowns[ability] - 1;

            if (remaining <= 0)
            {
                Cooldowns.Remove(ability);
            }
            else
            {
                Cooldowns[ability] = remaining;
            }
        }
    }
}
