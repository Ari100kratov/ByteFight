using Application.Abstractions.Messaging;

namespace Application.Game.Characters.GetByCurrentUserId;

public sealed record GetCharactersByCurrentUserIdQuery : IQuery<IReadOnlyList<CharacterResponse>>;
