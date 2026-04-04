namespace GameRuntime.Logic.User.Execution;

public sealed class UserCodeExecutionOptions
{
    /// <summary>
    /// Таймаут выполнения одного хода пользовательского кода.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(3);
}
