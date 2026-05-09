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
            SourceCode = @"// Это шаблон поведения персонажа.
// Каждый ход игра вызывает этот код и ожидает одно действие:
// Например: Attack, MoveTowards, MoveAwayFrom, Idle.

// 1. Оцени ситуацию на арене:
// - где находится персонаж
// - какие враги живы
// - кто рядом
// - кого можно атаковать
// - куда лучше двигаться

var enemies = world.AliveEnemies;

// Если врагов нет — ничего не делаем
if (!enemies.Any())
{
    return new Idle();
}

// 2. Выбери цель.
// Подумай, что важнее в этой стратегии:
// - атаковать ближайшего?
// - добивать врага с малым здоровьем?
// - держаться подальше от опасного врага?
// - сначала уничтожать особые цели?

var target = enemies
    // TODO: выбери подходящую сортировку или условие
    .FirstOrDefault();

// Если цель не выбрана — ничего не делаем
if (target is null)
{
    return new Idle();
}

// 3. Реши, что делать с выбранной целью.
// Подумай:
// - можно ли атаковать прямо сейчас?
// - нужно ли приблизиться?
// - нужно ли отступить?
// - стоит ли пропустить ход?

// TODO: добавь проверку возможности атаки
// Пример идеи:
// if (...)
// {
//     return new Attack(target.Id);
// }

// TODO: выбери подходящее движение
// Пример идеи:
// return new MoveTowards(target.Id);

return new Idle();"
        };

        return Task.FromResult(Result.Success(codeTemplate));
    }
}
