using Application.Abstractions.Messaging;
using Application.Contracts.GameRuntime;

namespace Application.GameRuntime.GameSessions.GetById;

public sealed record GetGameSessionByIdQuery(Guid Id) : IQuery<GameSessionDto>;
