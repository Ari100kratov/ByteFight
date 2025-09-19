using Application.Abstractions.Messaging;
using Application.Game.Characters;

namespace Application.Game.Characters.GetById;

public sealed record GetCharacterByIdQuery(Guid Id) : IQuery<CharacterResponse>;
