using Application.Abstractions.Messaging;
using Application.Game.Arenas.Enemies.Models;

namespace Application.Game.Arenas.Enemies.Add;

public sealed record AddEnemyToArenaCommand(
    Guid ArenaId,
    Guid EnemyId,
    PositionDto Position
) : ICommand<Guid>;
