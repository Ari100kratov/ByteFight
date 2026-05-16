namespace SharedKernel.Messaging;

/// <summary>
/// Обработчик запроса.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResponse">Тип результата.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Обрабатывает запрос.
    /// </summary>
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
