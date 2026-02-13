using GameRuntime.World;
using GameRuntime.World.Units;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Снимок состояния мира, доступный пользовательскому скрипту в методе <c>Decide</c>.
/// </summary>
public sealed class UserWorldView
{
    /// <summary>
    /// Текущий управляемый игроком юнит.
    /// </summary>
    public required UserUnitView Self { get; init; }

    /// <summary>
    /// Список вражеских юнитов на арене.
    /// </summary>
    public required IReadOnlyList<UserUnitView> Enemies { get; init; }

    /// <summary>
    /// Порядковый номер текущего хода.
    /// </summary>
    public int TurnIndex { get; init; }
}

internal static partial class Mapper
{
    public static UserWorldView ToView(this ArenaWorld world, BaseUnit actor)
    {
        return new UserWorldView
        {
            TurnIndex = world.TurnIndex,
            Self = new UserUnitView
            {
                Id = actor.Id,
                Position = actor.Position,
                Stats = new UserStatsView(actor.Stats),
                IsDead = actor.IsDead
            },
            Enemies = [.. world.Enemies
                .Select(enemy => new UserUnitView
                {
                    Id = enemy.Id,
                    Position = enemy.Position,
                    Stats = new UserStatsView(enemy.Stats),
                    IsDead = enemy.IsDead
                })]
        };
    }
}
