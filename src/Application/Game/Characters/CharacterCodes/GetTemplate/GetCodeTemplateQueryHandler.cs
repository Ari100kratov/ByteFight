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
            SourceCode = @"var target = world.Enemies
    .Where(e => !e.IsDead)
    .OrderBy(e => e.Position.ManhattanDistance(world.Self.Position))
    .FirstOrDefault();

if (target is null)
{
    return new Idle();
}

var distance = target.Position.ManhattanDistance(world.Self.Position);
var attackRange = world.Self.Stats.Get(StatType.AttackRange) ?? 1;

if (distance <= attackRange)
{
    return new Attack(target.Id);
}

return new MoveTo(target.Position);"
        };

        return Task.FromResult(Result.Success(codeTemplate));
    }
}
