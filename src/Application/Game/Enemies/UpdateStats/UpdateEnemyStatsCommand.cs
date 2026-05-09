using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.Enemies.UpdateStats;

public sealed record UpdateEnemyStatsCommand(Guid Id, IReadOnlyList<StatDto> Stats) : ICommand;
