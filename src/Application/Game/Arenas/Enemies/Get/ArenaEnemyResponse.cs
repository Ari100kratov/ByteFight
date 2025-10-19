using Application.Game.Arenas.Enemies.Models;

namespace Application.Game.Arenas.Enemies.Get;

public sealed record ArenaEnemyResponse(Guid Id, Guid EnemyId, string EnemyName, PositionDto Position);
