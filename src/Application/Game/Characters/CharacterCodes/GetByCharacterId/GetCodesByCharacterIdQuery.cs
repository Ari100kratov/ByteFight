using Application.Abstractions.Messaging;

namespace Application.Game.Characters.CharacterCodes.GetByCharacterId;

public sealed record GetCodesByCharacterIdQuery(Guid CharacterId) : IQuery<IReadOnlyList<CharacterCodeResponse>>;
