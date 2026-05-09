using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.CharacterSpecs.UpdateStats;

public sealed record UpdateSpecStatsCommand(Guid Id, IReadOnlyList<StatDto> Stats) : ICommand;
