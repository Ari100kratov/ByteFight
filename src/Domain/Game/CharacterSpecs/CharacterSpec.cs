using Domain.Game.CharacterClasses;
using Domain.Game.Characters;
using SharedKernel;

namespace Domain.Game.CharacterSpecs;

public sealed class CharacterSpec : Entity
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string Name { get; set; }
    public CharacterSpecType Type { get; set; }
    public string? Description { get; set; }

    public CharacterClass Class { get; set; }
    public IReadOnlyCollection<CharacterSpecStat> Stats { get; set; }
    public IReadOnlyCollection<CharacterSpecActionAsset> ActionAssets { get; set; }
    public IReadOnlyCollection<Character> Characters { get; set; }
}
