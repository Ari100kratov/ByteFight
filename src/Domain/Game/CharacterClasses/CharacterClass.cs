using Domain.Game.CharacterSpecs;
using SharedKernel;

namespace Domain.Game.CharacterClasses;

public class CharacterClass : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CharacterClassType Type { get; set; }
    public string? Description { get; set; }

    public IReadOnlyCollection<CharacterSpec> Specs { get; set; }
}
