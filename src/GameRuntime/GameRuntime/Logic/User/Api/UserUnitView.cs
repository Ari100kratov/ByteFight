using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

public sealed class UserUnitView
{
    public Guid Id { get; init; }
    public Position Position { get; init; }
    public UserStatsView Stats { get; init; }
    public bool IsDead { get; init; }
}
