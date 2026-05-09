using Domain.Game.Abilities;
using Domain.Game.CharacterSpecs;

namespace Domain.Game.CharacterSpecAbilities;

/// <summary>
/// Описывает способность специализации персонажа.
/// </summary>
public sealed class CharacterSpecAbility
{
    /// <summary>
    /// Идентификатор способности.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор специализации персонажа.
    /// </summary>
    public Guid CharacterSpecId { get; set; }

    /// <summary>
    /// Специализация персонажа, которой принадлежит способность.
    /// </summary>
    public CharacterSpec CharacterSpec { get; set; }

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
    public IReadOnlyCollection<CharacterSpecAbilityStat> Stats { get; set; }

    /// <summary>
    /// Анимации способности.
    /// </summary>
    public IReadOnlyCollection<CharacterSpecAbilityActionAsset> ActionAssets { get; set; }
}
