namespace GameRuntime.Logic.User.Api.Infrastructure;

public sealed record WorkerInput(
    string AssemblyPath,
    UserWorldView World);
