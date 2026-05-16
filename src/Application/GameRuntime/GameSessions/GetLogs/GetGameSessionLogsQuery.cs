
using Application.Contracts.GameRuntime;
using SharedKernel.Messaging;

namespace Application.GameRuntime.GameSessions.GetLogs;

public sealed record GetGameSessionLogsQuery(Guid SessionId)
    : IQuery<IReadOnlyList<TurnLogDto>>;
