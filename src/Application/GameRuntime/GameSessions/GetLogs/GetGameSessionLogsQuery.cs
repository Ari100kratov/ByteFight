using Application.Abstractions.Messaging;
using Application.Contracts.GameRuntime;

namespace Application.GameRuntime.GameSessions.GetLogs;

public sealed record GetGameSessionLogsQuery(Guid SessionId)
    : IQuery<IReadOnlyList<TurnLogDto>>;
