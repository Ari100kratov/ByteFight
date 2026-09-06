using Domain.Game.Abilities;
using Domain.GameRuntime.GameActionLogs;

namespace Application.Contracts.GameRuntime;

/// <summary>
/// Полный снимок состояния боя для клиента: раунд, очередь ходов,
/// юниты с ресурсами, статусами и способностями, поле с рельефом.
/// </summary>
public sealed record BattleStateDto
{
    public required int Round { get; init; }
    public required Guid? ActiveUnitId { get; init; }
    public required IReadOnlyList<Guid> TurnOrder { get; init; }
    public required IReadOnlyList<BattleUnitDto> Units { get; init; }
    public required BattleArenaDto Arena { get; init; }
}

/// <summary>
/// Юнит в снимке боя.
/// </summary>
public sealed record BattleUnitDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsPlayerSide { get; init; }
    public required bool IsDead { get; init; }
    public required PositionDto Position { get; init; }
    public required FacingDirection Facing { get; init; }

    public required decimal Health { get; init; }
    public required decimal MaxHealth { get; init; }
    public required decimal Mana { get; init; }
    public required decimal MaxMana { get; init; }
    public required int MovePoints { get; init; }
    public required int Actions { get; init; }
    public required int Initiative { get; init; }

    public required IReadOnlyList<BattleStatusDto> Statuses { get; init; }
    public required IReadOnlyDictionary<int, int> Cooldowns { get; init; }
    public required IReadOnlyList<BattleAbilityDto> Abilities { get; init; }
}

/// <summary>
/// Активный статус-эффект юнита.
/// </summary>
public sealed record BattleStatusDto(int Type, int Turns, decimal Magnitude);

/// <summary>
/// Способность юнита в снимке боя.
/// </summary>
public sealed record BattleAbilityDto
{
    public required AbilityType Type { get; init; }
    public required string Name { get; init; }
    public required AbilityEffectType EffectType { get; init; }
    public required AbilityTargetType TargetType { get; init; }
    public required AbilityShape Shape { get; init; }
    public required int Range { get; init; }
    public required int AreaRadius { get; init; }
    public required decimal Damage { get; init; }
    public required decimal Healing { get; init; }
    public required decimal ManaCost { get; init; }
    public required int Cooldown { get; init; }
    public required int ActionCost { get; init; }
    public required bool DashesToTarget { get; init; }
    public required IReadOnlyList<BattleAbilityStatusDto> Statuses { get; init; }
}

/// <summary>
/// Статус-эффект, накладываемый способностью.
/// </summary>
public sealed record BattleAbilityStatusDto(int Type, int Duration, decimal Magnitude);

/// <summary>
/// Поле боя с рельефом.
/// </summary>
public sealed record BattleArenaDto
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required IReadOnlyList<PositionDto> Blocked { get; init; }
    public required IReadOnlyList<BattleTerrainCellDto> Terrain { get; init; }
}

/// <summary>
/// Гекс с особым рельефом.
/// </summary>
public sealed record BattleTerrainCellDto(int X, int Y, int Terrain);
