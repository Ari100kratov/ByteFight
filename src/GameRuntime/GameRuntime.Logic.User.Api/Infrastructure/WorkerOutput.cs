namespace GameRuntime.Logic.User.Api.Infrastructure;

public sealed record WorkerOutput(
    bool Success,
    UserAction? Action,
    string? Error);
