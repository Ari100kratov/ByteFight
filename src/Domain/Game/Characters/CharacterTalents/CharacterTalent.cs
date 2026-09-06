using SharedKernel;

namespace Domain.Game.Characters.CharacterTalents;

/// <summary>
/// Выбранный талант персонажа с числом рангов.
/// Идентификатор узла ссылается на <see cref="Talents.TalentCatalog"/>.
/// </summary>
public sealed class CharacterTalent : Entity
{
    /// <summary>
    /// Уникальный идентификатор записи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор персонажа.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Персонаж.
    /// </summary>
    public Character Character { get; set; } = null!;

    /// <summary>
    /// Идентификатор узла таланта из каталога.
    /// </summary>
    public string TalentId { get; set; } = null!;

    /// <summary>
    /// Число вложенных рангов.
    /// </summary>
    public int Rank { get; set; }
}
