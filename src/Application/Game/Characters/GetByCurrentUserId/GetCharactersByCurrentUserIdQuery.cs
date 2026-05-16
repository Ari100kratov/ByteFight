
using SharedKernel.Messaging;

namespace Application.Game.Characters.GetByCurrentUserId;

public sealed record GetCharactersByCurrentUserIdQuery : IQuery<IReadOnlyList<CharacterResponse>>;
