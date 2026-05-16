
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.Game.Enemies.UpdateStats;

public sealed record UpdateEnemyStatsCommand(Guid Id, IReadOnlyList<StatDto> Stats) : ICommand;
