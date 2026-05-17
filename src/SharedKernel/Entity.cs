namespace SharedKernel;

/// <summary>
/// Базовый тип доменной сущности с буфером доменных событий.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Снимок накопленных доменных событий.
    /// </summary>
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    /// <summary>
    /// Очищает накопленные доменные события после их публикации.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Добавляет доменное событие в буфер сущности.
    /// </summary>
    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
