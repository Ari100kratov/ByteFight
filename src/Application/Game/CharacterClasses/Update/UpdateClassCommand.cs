using Application.Abstractions.Messaging;

namespace Application.Game.CharacterClasses.Update;

public sealed record UpdateClassCommand(
    Guid Id,
    string Name,
    string? Description
) : ICommand;
