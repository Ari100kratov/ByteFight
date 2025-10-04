using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Game.CharacterCodes.GetTemplate;

internal sealed class GetCodeTemplateQueryHandler : IQueryHandler<GetCodeTemplateQuery, CodeTemplateResponse>
{
    public Task<Result<CodeTemplateResponse>> Handle(GetCodeTemplateQuery query, CancellationToken cancellationToken)
    {
        var codeTemplate = new CodeTemplateResponse
        {
            Id = Guid.CreateVersion7(),
            Name = "Новый код",
            SourceCode = @"using System;

public class Character
{
    public void Act()
    {
        // Напишите здесь поведение персонажа
        Console.WriteLine(""Действие персонажа!"");
    }
}"
        };


        return Task.FromResult(Result.Success(codeTemplate));
    }
}
