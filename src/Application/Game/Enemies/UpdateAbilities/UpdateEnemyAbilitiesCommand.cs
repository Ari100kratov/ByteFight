using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.Enemies.UpdateAbilities;

public sealed record UpdateEnemyAbilitiesCommand(Guid Id, IReadOnlyList<AbilityDto> Abilities) : ICommand;
