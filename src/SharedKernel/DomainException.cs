namespace SharedKernel;

/// <summary>
/// Исключение для нарушения доменных инвариантов с устойчивым кодом ошибки.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Машиночитаемый код доменной ошибки.
    /// </summary>
    public string Code { get; }

    public DomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public DomainException(string code, string message, Exception inner)
        : base(message, inner)
    {
        Code = code;
    }
}
