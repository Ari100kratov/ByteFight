namespace Application.Game.Characters.GetById;

public sealed record CharacterResponse(Guid Id, string Name, Guid ClassId);
