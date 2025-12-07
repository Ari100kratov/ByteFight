using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.Game.Arenas.Enemies.Add;

public sealed record AddEnemyToArenaCommand(
    Guid ArenaId,
    Guid EnemyId,
    PositionDto Position
) : ICommand<Guid>;
