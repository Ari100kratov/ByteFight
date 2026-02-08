using GameRuntime.Core;
using GameRuntime.Core.Units;

namespace GameRuntime.Logic.User.Api;

public sealed class UserWorldView
{
    public required UserUnitView Self { get; init; }
    public required IReadOnlyList<UserUnitView> Enemies { get; init; }
    public int TurnIndex { get; init; }
}

public static partial class Mapper
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
