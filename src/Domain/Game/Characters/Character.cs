using SharedKernel;

namespace Domain.Game.Characters;

public sealed class Character : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt {  get; set; }

    public UserId UserId { get; set; }
}
