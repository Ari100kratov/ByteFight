namespace SharedKernel;

/// <summary>
/// Помечает тип как доступный для пользовательского API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
public sealed class UserCodeApiAttribute : Attribute { }
