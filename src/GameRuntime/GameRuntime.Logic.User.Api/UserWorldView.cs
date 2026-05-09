using Domain.Game.Abilities;
using Domain.Game.Stats;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;
using SharedKernel;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Снимок состояния мира, доступный пользовательскому скрипту в методе <c>Decide</c>.
/// </summary>
[UserCodeApi]
public sealed class UserWorldView
{
    /// <summary>
    /// Информация об игровой арене
    /// </summary>
    public required UserArenaDefinition Arena { get; init; }

    /// <summary>
    /// Текущий управляемый игроком юнит.
    /// </summary>
    public required UserUnitView Self { get; init; }

    /// <summary>
    /// Список вражеских юнитов на арене.
    /// </summary>
    public required IReadOnlyList<UserUnitView> Enemies { get; init; }

    /// <summary>
    /// Список живых вражеских юнитов.
    /// </summary>
    public IEnumerable<UserUnitView> AliveEnemies => Enemies.Where(e => e.IsAlive);

    /// <summary>
    /// Порядковый номер текущего хода.
    /// </summary>
    public int TurnIndex { get; init; }

    /// <summary>
    /// Проверяет, занята ли клетка живым юнитом.
    ///
    /// Учитывает как управляемого игроком юнита, так и живых врагов.
    /// </summary>
    /// <param name="position">Позиция для проверки.</param>
    public bool IsOccupied(Position position)
    {
        if (Self.Position == position)
        {
            return true;
        }

        return Enemies.Any(e => e.IsAlive && e.Position == position);
    }

    /// <summary>
    /// Проверяет, можно ли перемещаться в указанную клетку.
    ///
    /// Клетка считается проходимой, если:
    /// она находится в пределах арены;
    /// не является статическим препятствием;
    /// не занята живым юнитом.
    /// </summary>
    /// <param name="position">Позиция для проверки.</param>
    public bool IsWalkable(Position position)
        => Arena.IsWithin(position)
           && !Arena.IsBlocked(position)
           && !IsOccupied(position);

    /// <summary>
    /// Возвращает соседние по ортогонали клетки, в которые можно перемещаться.
    /// </summary>
    /// <param name="position">Центральная позиция.</param>
    public IEnumerable<Position> GetWalkableNeighbors4(Position position)
        => Arena.GetNeighbors4(position).Where(IsWalkable);

    /// <summary>
    /// Возвращает соседние по ортогонали клетки для позиции текущего игрока,
    /// в которые можно перемещаться.
    /// </summary>
    public IEnumerable<Position> GetWalkableNeighbors4()
        => GetWalkableNeighbors4(Self.Position);
}

public static partial class Mapper
{
    public static UserWorldView ToView(this ArenaWorld world, BaseUnit actor)
    {
        return new UserWorldView
        {
            TurnIndex = world.TurnIndex,

            Arena = new UserArenaDefinition
            {
                GridWidth = world.Arena.GridWidth,
                GridHeight = world.Arena.GridHeight,
                StartPosition = world.Arena.StartPosition,
                BlockedPositions = world.Arena.BlockedPositions
            },

            Self = actor.ToUserUnitView(),

            Enemies = [.. world.Enemies.Select(enemy => enemy.ToUserUnitView())]
        };
    }

    private static UserUnitView ToUserUnitView(this BaseUnit unit)
    {
        return new UserUnitView
        {
            Id = unit.Id,
            Name = unit.Name,
            Position = unit.Position,
            IsDead = unit.IsDead,

            Stats = new UserStatsView
            {
                Current = new Dictionary<StatType, decimal>(unit.Stats.Current),
                Max = new Dictionary<StatType, decimal>(unit.Stats.Maximum)
            },

            Abilities = unit.Abilities.ToUserAbilitiesView()
        };
    }

    private static UserAbilitiesView ToUserAbilitiesView(this RuntimeAbilities abilities)
    {
        return new UserAbilitiesView
        {
            All = [.. abilities.All
                .OrderByDescending(x => x.Priority)
                .Select(x => new UserAbilityView
                {
                    Type = x.Type,
                    EffectType = x.EffectType,
                    TargetType = x.TargetType,
                    Name = x.Name,
                    Priority = x.Priority,
                    Stats = new Dictionary<AbilityStatType, decimal>(x.Stats)
                })]
        };
    }
}
