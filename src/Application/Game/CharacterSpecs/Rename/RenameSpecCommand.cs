using Application.Abstractions.Messaging;

namespace Application.Game.CharacterSpecs.Rename;

public sealed record RenameSpecCommand(Guid Id, string Name) : ICommand;
