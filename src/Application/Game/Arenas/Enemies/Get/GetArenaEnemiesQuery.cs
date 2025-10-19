using Application.Abstractions.Messaging;

namespace Application.Game.Arenas.Enemies.Get;

public sealed record GetArenaEnemiesQuery(Guid ArenaId) : IQuery<IReadOnlyList<ArenaEnemyResponse>>;
