namespace SharedKernel;

/// <summary>
/// Ошибка валидации, агрегирующая несколько ошибок отдельных правил.
/// </summary>
public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    /// <summary>
    /// Ошибки отдельных правил валидации.
    /// </summary>
    public Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new([.. results.Where(r => r.IsFailure).Select(r => r.Error)]);
}
