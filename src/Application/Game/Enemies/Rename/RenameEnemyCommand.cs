
using SharedKernel.Messaging;

namespace Application.Game.Enemies.Rename;

public sealed record RenameEnemyCommand(Guid Id, string Name) : ICommand;
