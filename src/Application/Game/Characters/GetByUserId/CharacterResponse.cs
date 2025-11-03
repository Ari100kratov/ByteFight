namespace Application.Game.Characters.GetByUserId;

public sealed record CharacterResponse(Guid Id, string Name, Guid ClassId);
