using System.Security.Cryptography;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Game.Characters.CharacterCodes.GetTemplate;

internal sealed class GetCodeTemplateQueryHandler
    : IQueryHandler<GetCodeTemplateQuery, CodeTemplateResponse>
{
    private static readonly string[] TemplateNames =
    [
        "works-on-my-machine.cs",
        "final-final-v2.cs",
        "definitely-not-bugged.cs",
        "temporary-solution.cs",
        "one-last-refactor.cs",
        "smart-ai.cs",
        "dont-touch-it.cs",
        "magic-happens-here.cs",
        "ship-it.cs",
        "todo-remove-later.cs",
        "berserk-mode.cs",
        "trust-the-algorithm.cs",
        "just-one-hotfix.cs",
        "probably-optimal.cs",
        "ai-core.cs",
        "this-should-work.cs",
        "brain.cs",
        "production-ready.cs",
    ];

    public Task<Result<CodeTemplateResponse>> Handle(
        GetCodeTemplateQuery query,
        CancellationToken cancellationToken)
    {
        int index = RandomNumberGenerator.GetInt32(TemplateNames.Length);

        var codeTemplate = new CodeTemplateResponse
        {
            Id = Guid.CreateVersion7(),
            Name = TemplateNames[index],

            SourceCode = """
// Берём только живых врагов и выбираем цель.
// Сначала приоритет у тех, кого уже можно атаковать.
// Если таких нет — выбираем ближайшего.
var target = world.AliveEnemies
    .OrderBy(e => world.Self.CanAttack(e) ? 0 : 1)
    .ThenBy(e => world.Self.DistanceTo(e))
    .FirstOrDefault();

// Если живых врагов нет — пропускаем ход.
if (target is null)
{
    return new Idle();
}

// Если цель уже в радиусе атаки — атакуем её.
if (world.Self.CanAttack(target))
{
    return new Attack(target.Id);
}

// Иначе двигаемся в сторону выбранной цели.
return new MoveTowards(target.Id);
"""
        };

        return Task.FromResult(Result.Success(codeTemplate));
    }
}
