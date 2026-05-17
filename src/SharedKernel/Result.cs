using System.Diagnostics.CodeAnalysis;

namespace SharedKernel;

/// <summary>
/// Представляет результат операции без возвращаемого значения.
/// </summary>
public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Возвращает <see langword="true" />, если операция завершилась успешно.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Возвращает <see langword="true" />, если операция завершилась ошибкой.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Ошибка операции или <see cref="Error.None" /> для успешного результата.
    /// </summary>
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);
}

/// <summary>
/// Представляет результат операции с возвращаемым значением.
/// </summary>
/// <typeparam name="TValue">Тип значения успешного результата.</typeparam>
public class Result<TValue> : Result
{
    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Значение успешного результата. Для неуспешного результата выбрасывает исключение.
    /// </summary>
    [NotNull]
    public TValue Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
