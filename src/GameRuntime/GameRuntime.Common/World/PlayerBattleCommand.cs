using Domain.Game.Abilities;
using Domain.ValueObjects;

namespace GameRuntime.Common.World;

/// <summary>
/// Команда игрока в ручном режиме боя.
/// </summary>
public abstract record PlayerBattleCommand;

/// <summary>
/// Перемещение по указанному пути (первый гекс — текущая позиция).
/// </summary>
/// <param name="Path">Путь перемещения.</param>
public sealed record MoveCommand(IReadOnlyList<Position> Path) : PlayerBattleCommand;

/// <summary>
/// Применение способности на целевой гекс.
/// </summary>
/// <param name="Ability">Тип способности.</param>
/// <param name="TargetHex">Гекс, на который применяется способность.</param>
public sealed record UseAbilityCommand(AbilityType Ability, Position TargetHex)
    : PlayerBattleCommand;

/// <summary>
/// Завершение хода досрочно.
/// </summary>
public sealed record EndTurnCommand : PlayerBattleCommand;
