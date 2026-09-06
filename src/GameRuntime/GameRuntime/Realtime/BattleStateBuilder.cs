using Application.Contracts;
using Application.Contracts.GameRuntime;
using Domain.Game.Abilities;
using Domain.Game.Stats;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Realtime;

/// <summary>
/// Строит полный снимок состояния боя для отправки клиенту.
/// </summary>
internal static class BattleStateBuilder
{
    public static BattleStateDto Build(ArenaWorld world)
    {
        return new BattleStateDto
        {
            Round = world.RoundNumber,
            ActiveUnitId = world.ActiveUnit?.Id,
            TurnOrder = [.. world.RoundOrder.Select(x => x.Value)],
            Units = [.. world.AllUnits.Select(unit => ToUnitDto(world, unit))],
            Arena = ToArenaDto(world)
        };
    }

    private static BattleUnitDto ToUnitDto(ArenaWorld world, BaseUnit unit)
    {
        return new BattleUnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            IsPlayerSide = world.IsPlayerSide(unit),
            IsDead = unit.IsDead,
            Position = new PositionDto(unit.Position.X, unit.Position.Y),
            Facing = unit.FacingDirection,
            Health = unit.Stats.GetHealth(),
            MaxHealth = unit.Stats.GetMaxHealth(),
            Mana = unit.Stats.Get(StatType.Mana),
            MaxMana = unit.Stats.GetMax(StatType.Mana),
            MovePoints = unit.BattleState.MovePointsRemaining,
            Actions = unit.BattleState.ActionsRemaining,
            Initiative = unit.Stats.GetInitiative(),
            Statuses = [.. unit.BattleState.Statuses.All.Select(s =>
                new BattleStatusDto((int)s.Type, s.RemainingTurns, s.Magnitude))],
            Cooldowns = unit.BattleState.Cooldowns.ToDictionary(x => (int)x.Key, x => x.Value),
            Abilities = [.. unit.Abilities.All.Select(ToAbilityDto)]
        };
    }

    private static BattleAbilityDto ToAbilityDto(RuntimeAbility ability) =>
        new()
        {
            Type = ability.Type,
            Name = ability.Name,
            EffectType = ability.EffectType,
            TargetType = ability.TargetType,
            Shape = ability.Shape,
            Range = ability.GetRange(),
            AreaRadius = ability.GetAreaRadius(),
            Damage = ability.Get(AbilityStatType.Damage),
            Healing = ability.Get(AbilityStatType.Healing),
            ManaCost = ability.GetManaCost(),
            Cooldown = ability.GetCooldown(),
            ActionCost = ability.GetActionCost(),
            DashesToTarget = ability.DashesToTarget,
            Statuses = [.. ability.AppliedStatuses.Select(s =>
                new BattleAbilityStatusDto((int)s.Type, s.Duration, s.Magnitude))]
        };

    private static BattleArenaDto ToArenaDto(ArenaWorld world) =>
        new()
        {
            Width = world.Arena.GridWidth,
            Height = world.Arena.GridHeight,
            Blocked = [.. world.Arena.BlockedPositions.Select(p => new PositionDto(p.X, p.Y))],
            Terrain = [.. world.Arena.Terrain.Select(kv =>
                new BattleTerrainCellDto(kv.Key.X, kv.Key.Y, (int)kv.Value))]
        };
}
