using Domain.Game.Characters;
using SharedKernel;

namespace Domain.Game.CharacterClasses;

public class CharacterClass : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CharacterClassType Type { get; set; }
    public string? Description { get; set; }

    public IReadOnlyCollection<CharacterClassActionAsset> ActionAssets { get; set; }
    public IReadOnlyCollection<CharacterClassStat> Stats { get; set; }
    public IReadOnlyCollection<Character> Characters { get; set; }
}
