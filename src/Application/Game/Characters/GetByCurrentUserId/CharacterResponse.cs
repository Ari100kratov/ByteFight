namespace Application.Game.Characters.GetByCurrentUserId;

public sealed record CharacterResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid UserId { get; init; }
}
