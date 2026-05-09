namespace GameRuntime.UserCodeApiDocumentation;

/// <summary>
/// Документация API, доступного пользовательскому коду.
/// </summary>
public sealed record UserCodeApiDoc
{
    /// <summary>
    /// Список публичных типов пользовательского API.
    /// </summary>
    public required IReadOnlyList<ApiTypeDoc> Types { get; init; }
}

/// <summary>
/// Документация одного типа пользовательского API.
/// </summary>
public sealed record ApiTypeDoc
{
    /// <summary>
    /// Имя типа без namespace.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Полное имя типа вместе с namespace.
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// Namespace типа.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// Вид типа: class, record, struct, enum.
    /// </summary>
    public required ApiTypeKind Kind { get; init; }

    /// <summary>
    /// XML summary типа.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Публичные свойства типа.
    /// </summary>
    public required IReadOnlyList<ApiPropertyDoc> Properties { get; init; }

    /// <summary>
    /// Публичные методы типа.
    /// </summary>
    public required IReadOnlyList<ApiMethodDoc> Methods { get; init; }

    /// <summary>
    /// Значения enum-типа.
    /// </summary>
    public required IReadOnlyList<ApiEnumValueDoc> EnumValues { get; init; }
}

/// <summary>
/// Вид типа пользовательского API.
/// </summary>
public enum ApiTypeKind
{
    /// <summary>
    /// Обычный class.
    /// </summary>
    Class = 1,

    /// <summary>
    /// Record class.
    /// </summary>
    Record = 2,

    /// <summary>
    /// Struct.
    /// </summary>
    Struct = 3,

    /// <summary>
    /// Enum.
    /// </summary>
    Enum = 4
}

/// <summary>
/// Документация свойства пользовательского API.
/// </summary>
public sealed record ApiPropertyDoc
{
    /// <summary>
    /// Имя свойства.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Отображаемое имя типа свойства.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Полное имя типа свойства, если доступно.
    /// </summary>
    public string? TypeFullName { get; init; }

    /// <summary>
    /// XML summary свойства.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Признак required-свойства.
    /// </summary>
    public required bool IsRequired { get; init; }

    /// <summary>
    /// Признак nullable-типа.
    /// </summary>
    public required bool IsNullable { get; init; }

    /// <summary>
    /// Признак вычисляемого свойства без public init/set.
    /// </summary>
    public required bool IsComputed { get; init; }
}

/// <summary>
/// Документация метода пользовательского API.
/// </summary>
public sealed record ApiMethodDoc
{
    /// <summary>
    /// Имя метода.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Отображаемый тип возвращаемого значения.
    /// </summary>
    public required string ReturnType { get; init; }

    /// <summary>
    /// Полное имя возвращаемого типа, если доступно.
    /// </summary>
    public string? ReturnTypeFullName { get; init; }

    /// <summary>
    /// XML summary метода.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// XML returns метода.
    /// </summary>
    public string? Returns { get; init; }

    /// <summary>
    /// Параметры метода.
    /// </summary>
    public required IReadOnlyList<ApiParameterDoc> Parameters { get; init; }
}

/// <summary>
/// Документация параметра метода пользовательского API.
/// </summary>
public sealed record ApiParameterDoc
{
    /// <summary>
    /// Имя параметра.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Отображаемый тип параметра.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Полное имя типа параметра, если доступно.
    /// </summary>
    public string? TypeFullName { get; init; }

    /// <summary>
    /// XML param summary.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Признак nullable-параметра.
    /// </summary>
    public required bool IsNullable { get; init; }

    /// <summary>
    /// Признак optional-параметра.
    /// </summary>
    public required bool IsOptional { get; init; }

    /// <summary>
    /// Значение по умолчанию, если параметр optional.
    /// </summary>
    public string? DefaultValue { get; init; }
}

/// <summary>
/// Документация значения enum пользовательского API.
/// </summary>
public sealed record ApiEnumValueDoc
{
    /// <summary>
    /// Имя значения enum.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Числовое значение enum.
    /// </summary>
    public required long Value { get; init; }

    /// <summary>
    /// XML summary значения enum.
    /// </summary>
    public string? Summary { get; init; }
}
