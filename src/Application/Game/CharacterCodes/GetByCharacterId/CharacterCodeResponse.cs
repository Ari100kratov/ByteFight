using Domain.Game.CharacterCodes;

namespace Application.Game.CharacterCodes.GetByCharacterId;

public sealed record CharacterCodeResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string? SourceCode { get; init; }
}
