using Domain.Game.Characters.CharacterCodes;
using Domain.Game.Characters.CharacterTalents;
using Domain.Game.CharacterSpecs;
using SharedKernel;

namespace Domain.Game.Characters;

public sealed class Character : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid SpecId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Уровень персонажа (1–30). Каждый уровень выше первого даёт очко талантов.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Накопленный опыт внутри текущего уровня.
    /// </summary>
    public int Experience { get; set; }

    public UserId UserId { get; set; }

    public CharacterSpec Spec { get; set; }
    public IReadOnlyCollection<CharacterCode> Codes { get; set; }

    /// <summary>
    /// Выбранные таланты персонажа.
    /// </summary>
    public IReadOnlyCollection<CharacterTalent> Talents { get; set; }

    /// <summary>
    /// Словарь выбранных талантов: идентификатор узла → ранг.
    /// </summary>
    public IReadOnlyDictionary<string, int> GetTalentRanks() =>
        Talents?.ToDictionary(x => x.TalentId, x => x.Rank)
            ?? new Dictionary<string, int>();
}
