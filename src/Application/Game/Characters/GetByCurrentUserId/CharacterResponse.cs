namespace Application.Game.Characters.GetByCurrentUserId;

public sealed record CharacterResponse(Guid Id, string Name, Guid ClassId);
