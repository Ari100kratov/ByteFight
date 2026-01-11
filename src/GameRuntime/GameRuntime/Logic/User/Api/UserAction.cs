using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

public abstract record UserAction;

public sealed record Attack(Guid TargetId) : UserAction;

public sealed record MoveTo(Position Target) : UserAction;

public sealed record Idle() : UserAction;
