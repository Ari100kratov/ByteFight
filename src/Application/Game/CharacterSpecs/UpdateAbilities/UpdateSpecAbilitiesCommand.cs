using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.CharacterSpecs.UpdateAbilities;

public sealed record UpdateSpecAbilitiesCommand(Guid Id, IReadOnlyList<AbilityDto> Abilities) : ICommand;
