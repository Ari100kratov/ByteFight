namespace SharedKernel;

/// <summary>
/// Машиночитаемое описание ошибки, возвращаемой application и presentation слоями.
/// </summary>
public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// Устойчивый код ошибки для логов, API и клиентской обработки.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Человекочитаемое описание ошибки.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Категория ошибки, определяющая обработку и HTTP-статус.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Создаёт ошибку общего сбоя.
    /// </summary>
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
}
