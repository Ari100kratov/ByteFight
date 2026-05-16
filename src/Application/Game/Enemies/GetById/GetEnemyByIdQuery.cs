
using SharedKernel.Messaging;

namespace Application.Game.Enemies.GetById;

public sealed record GetEnemyByIdQuery(Guid Id) : IQuery<EnemyResponse>;
