using Application.Abstractions.Messaging;

namespace Application.Game.CharacterSpecs.GetSpecsByClassId;

public sealed record GetSpecsByClassIdQuery(Guid ClassId) : IQuery<IReadOnlyList<SpecResponse>>;
