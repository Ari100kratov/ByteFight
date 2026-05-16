
using SharedKernel.Messaging;

namespace Application.Game.Enemies.UpdateDescription;

public sealed record UpdateEnemyDescriptionCommand(Guid Id, string? Description) : ICommand;
