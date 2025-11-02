using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Game.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterClasses.GetAll;

internal sealed class GetAllClassesQueryHandler(IGameDbContext dbContext)
    : IQueryHandler<GetAllClassesQuery, IReadOnlyList<ClassResponse>>
{
    public async Task<Result<IReadOnlyList<ClassResponse>>> Handle(GetAllClassesQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<ClassResponse> characterClasses = await dbContext.CharacterClasses
            .Select(x => new ClassResponse(
                x.Id,
                x.Name,
                x.Type,
                x.Description,
                x.Stats.Select(s => s.ToDto()).ToArray(),
                x.ActionAssets.Select(a => a.ToDto()).ToArray()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(characterClasses);
    }
}
