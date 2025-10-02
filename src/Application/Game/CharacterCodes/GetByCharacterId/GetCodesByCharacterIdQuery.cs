using Application.Abstractions.Messaging;

namespace Application.Game.CharacterCodes.GetByCharacterId;

public sealed record GetCodesByCharacterIdQuery(Guid CharacterId) : IQuery<IReadOnlyList<CharacterCodeResponse>>;
