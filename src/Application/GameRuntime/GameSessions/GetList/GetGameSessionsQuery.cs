
using Application.Contracts;
using SharedKernel.Messaging;

namespace Application.GameRuntime.GameSessions.GetList;

public sealed record GetGameSessionsQuery(
    int Page = 1,
    int PageSize = 20)
    : IQuery<PagedResponse<GameSessionListItemDto>>;
