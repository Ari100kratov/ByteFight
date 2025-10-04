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
            Name = "����� ���",
            SourceCode = @"using System;

public class Character
{
    public void Act()
    {
        // �������� ����� ��������� ���������
        Console.WriteLine(""�������� ���������!"");
    }
}"
        };


        return Task.FromResult(Result.Success(codeTemplate));
    }
}
