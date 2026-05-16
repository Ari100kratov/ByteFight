
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.CharacterSpecs.UpdateStats;

public sealed record UpdateSpecStatsCommand(Guid Id, IReadOnlyList<StatDto> Stats) : ICommand;
