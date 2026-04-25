using Application.Abstractions.Messaging;
using Application.Contracts;

namespace Application.GameRuntime.GameSessions.GetList;

public sealed record GetGameSessionsQuery(
    int Page = 1,
    int PageSize = 20)
    : IQuery<PagedResponse<GameSessionListItemDto>>;
