using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Представление юнита в пользовательском API.
/// </summary>
public sealed class UserUnitView
{
    /// <summary>
    /// Уникальный идентификатор юнита.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Текущая позиция юнита на арене.
    /// </summary>
    public Position Position { get; init; }

    /// <summary>
    /// Снимок характеристик юнита.
    /// </summary>
    public UserStatsView Stats { get; init; }

    /// <summary>
    /// Признак того, что юнит мёртв.
    /// </summary>
    public bool IsDead { get; init; }
}
