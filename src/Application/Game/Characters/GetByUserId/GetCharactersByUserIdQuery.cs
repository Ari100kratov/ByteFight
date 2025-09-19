using Application.Abstractions.Messaging;
using Application.Game.Characters;

namespace Application.Game.Characters.GetByUserId;

public sealed record GetCharactersByUserIdQuery(Guid UserId) : IQuery<IReadOnlyList<CharacterResponse>>;
