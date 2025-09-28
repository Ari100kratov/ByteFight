using Application.Abstractions.Messaging;

namespace Application.Game.Characters.GetByUserId;

public sealed record GetCharactersByUserIdQuery(Guid UserId) : IQuery<IReadOnlyList<CharacterResponse>>;
