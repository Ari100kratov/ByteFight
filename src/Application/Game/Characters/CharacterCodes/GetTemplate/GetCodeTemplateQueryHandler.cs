using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Game.Characters.CharacterCodes.GetTemplate;

internal sealed class GetCodeTemplateQueryHandler : IQueryHandler<GetCodeTemplateQuery, CodeTemplateResponse>
{
    public Task<Result<CodeTemplateResponse>> Handle(GetCodeTemplateQuery query, CancellationToken cancellationToken)
    {
        var codeTemplate = new CodeTemplateResponse
        {
            Id = Guid.CreateVersion7(),
            Name = "Program.cs",
            SourceCode = @"// Выбираем цель:
// 1. Сначала тех, кто уже в радиусе атаки
// 2. Затем с наименьшим здоровьем
// 3. Затем ближайшего
var target = world.AliveEnemies
    .OrderBy(e => world.Self.IsInAttackRange(e) ? 0 : 1)
    .ThenBy(e => e.Health)
    .ThenBy(e => world.Self.DistanceTo(e))
    .FirstOrDefault();

// Если врагов нет — ничего не делаем
if (target is null)
{
    return new Idle();
}

// Если можем атаковать — атакуем
if (world.Self.IsInAttackRange(target))
{
    return new Attack(target.Id);
}

// Иначе двигаемся к цели
return new MoveTowards(target.Id);"
        };

        return Task.FromResult(Result.Success(codeTemplate));
    }
}
