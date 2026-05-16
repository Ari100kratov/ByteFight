namespace SharedKernel.Messaging;

/// <summary>
/// Запрос с возвращаемым значением.
/// </summary>
/// <typeparam name="TResponse">Тип результата запроса.</typeparam>
public interface IQuery<TResponse>;
