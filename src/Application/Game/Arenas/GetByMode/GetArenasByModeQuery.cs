
using Domain.Game.GameModes;
using SharedKernel.Messaging;

namespace Application.Game.Arenas.GetByMode;

public sealed record GetArenasByModeQuery(GameModeType Mode) : IQuery<IReadOnlyList<ArenaResponse>>;
