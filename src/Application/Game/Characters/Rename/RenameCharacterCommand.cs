using Application.Abstractions.Messaging;

namespace Application.Game.Characters.Rename;

public sealed record RenameCharacterCommand(Guid Id, string Name) : ICommand;
