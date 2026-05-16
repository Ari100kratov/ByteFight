
using Application.Contracts.GameRuntime;
using SharedKernel.Messaging;

namespace Application.GameRuntime.GameSessions.GetById;

public sealed record GetGameSessionByIdQuery(Guid Id) : IQuery<GameSessionDto>;
