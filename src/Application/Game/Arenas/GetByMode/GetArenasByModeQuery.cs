using Application.Abstractions.Messaging;
using Domain.Game.GameModes;

namespace Application.Game.Arenas.GetByMode;

public sealed record GetArenasByModeQuery(GameModeType Mode) : IQuery<IReadOnlyList<ArenaResponse>>;
