
using SharedKernel.Messaging;

namespace Application.Game.CharacterSpecs.Rename;

public sealed record RenameSpecCommand(Guid Id, string Name) : ICommand;
