
using SharedKernel.Messaging;

namespace Application.Game.Characters.GetById;

public sealed record GetCharacterByIdQuery(Guid Id) : IQuery<CharacterResponse>;
