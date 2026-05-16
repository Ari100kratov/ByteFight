namespace SharedKernel.Messaging;

/// <summary>
/// Обработчик команды без возвращаемого значения.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Обрабатывает команду.
    /// </summary>
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Обработчик команды с возвращаемым значением.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
/// <typeparam name="TResponse">Тип результата.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Обрабатывает команду.
    /// </summary>
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}
