using Domain.Game.Characters.CharacterCodes;
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

    public UserId UserId { get; set; }

    public CharacterSpec Spec { get; set; }
    public IReadOnlyCollection<CharacterCode> Codes { get; set; }
}
