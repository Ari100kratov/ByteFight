
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.CharacterSpecs.UpdateAbilities;

public sealed record UpdateSpecAbilitiesCommand(Guid Id, IReadOnlyList<AbilityDto> Abilities) : ICommand;
