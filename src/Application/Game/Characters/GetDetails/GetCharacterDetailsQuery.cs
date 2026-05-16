
using SharedKernel.Messaging;

namespace Application.Game.Characters.GetDetails;

public sealed record GetCharacterDetailsQuery(Guid Id) : IQuery<CharacterResponse>;
