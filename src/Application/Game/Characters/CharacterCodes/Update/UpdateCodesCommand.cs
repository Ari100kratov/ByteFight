using Application.Abstractions.Messaging;

namespace Application.Game.Characters.CharacterCodes.Update;

public sealed record UpdateCodesCommand(
    Guid CharacterId,
    IReadOnlyList<CharacterCodeDto> Created,
    IReadOnlyList<CharacterCodeDto> Updated,
    IReadOnlyList<Guid> DeletedIds
) : ICommand;

public sealed record CharacterCodeDto(
    Guid Id,
    string Name,
    string? SourceCode
);
