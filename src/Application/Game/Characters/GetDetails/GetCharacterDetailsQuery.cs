using Application.Abstractions.Messaging;

namespace Application.Game.Characters.GetDetails;

public sealed record GetCharacterDetailsQuery(Guid Id) : IQuery<CharacterResponse>;
