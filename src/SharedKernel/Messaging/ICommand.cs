namespace SharedKernel.Messaging;

/// <summary>
/// Команда без возвращаемого значения.
/// </summary>
public interface ICommand;

/// <summary>
/// Команда с возвращаемым значением.
/// </summary>
/// <typeparam name="TResponse">Тип результата команды.</typeparam>
public interface ICommand<TResponse>;
