using Domain.Game.Abilities;
using SharedKernel;

namespace Domain.Game.Enemies;

/// <summary>
/// Описывает способность противника.
/// </summary>
public sealed class EnemyAbility : Entity
{
    /// <summary>
    /// Идентификатор способности.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор противника.
    /// </summary>
    public Guid EnemyId { get; set; }

    /// <summary>
    /// Противник, которому принадлежит способность.
    /// </summary>
    public Enemy Enemy { get; set; }

    /// <summary>
    /// Тип способности.
    /// </summary>
    public AbilityType Type { get; set; }

    /// <summary>
    /// Тип эффекта способности.
    /// </summary>
    public AbilityEffectType EffectType { get; set; }

    /// <summary>
    /// Тип цели способности.
    /// </summary>
    public AbilityTargetType TargetType { get; set; }

    /// <summary>
    /// Название способности.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание способности.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Приоритет выбора способности.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Характеристики способности.
    /// </summary>
    public ICollection<EnemyAbilityStat> Stats { get; set; }

    /// <summary>
    /// Анимации способности.
    /// </summary>
    public ICollection<EnemyAbilityActionAsset> ActionAssets { get; set; }
}
