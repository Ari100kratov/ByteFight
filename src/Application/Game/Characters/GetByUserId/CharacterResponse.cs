namespace Application.Game.Characters.GetByUserId;

public sealed record CharacterResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid UserId { get; init; }
}
