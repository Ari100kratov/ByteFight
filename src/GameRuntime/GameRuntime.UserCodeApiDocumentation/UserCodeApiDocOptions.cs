using System.Reflection;

namespace GameRuntime.UserCodeApiDocumentation;

/// <summary>
/// Настройки генерации документации пользовательского API.
/// </summary>
public sealed class UserCodeApiDocOptions
{
    /// <summary>
    /// Сборки, в которых нужно искать типы с UserCodeApiAttribute.
    /// </summary>
    public required IReadOnlyList<Assembly> Assemblies { get; set; }
}
