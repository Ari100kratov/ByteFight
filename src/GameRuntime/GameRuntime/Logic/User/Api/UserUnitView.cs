using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Юнит.
/// </summary>
public sealed class UserUnitView
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Текущая позиция на арене.
    /// </summary>
    public Position Position { get; init; }

    /// <summary>
    /// Характеристики юнита.
    /// </summary>
    public UserStatsView Stats { get; init; }

    /// <summary>
    /// Признак того, что юнит мёртв.
    /// </summary>
    public bool IsDead { get; init; }
}
