
using SharedKernel.Messaging;

namespace Application.Game.Arenas.GetById;

public sealed record GetArenaByIdQuery(Guid Id) : IQuery<ArenaResponse>;
