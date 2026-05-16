
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.Enemies.UpdateAbilities;

public sealed record UpdateEnemyAbilitiesCommand(Guid Id, IReadOnlyList<AbilityDto> Abilities) : ICommand;
