using Application.Abstractions.Messaging;

namespace Application.Game.Arenas.Enemies.Remove;

public sealed record RemoveEnemyFromArenaCommand(Guid ArenaId, Guid ArenaEnemyId) : ICommand;
