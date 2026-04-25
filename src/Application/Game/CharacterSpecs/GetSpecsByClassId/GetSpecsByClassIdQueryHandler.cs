using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.CharacterSpecs.GetSpecsByClassId;

internal sealed class GetSpecsByClassIdQueryHandler(IGameDbContext dbContext)
    : IQueryHandler<GetSpecsByClassIdQuery, IReadOnlyList<SpecResponse>>
{
    public async Task<Result<IReadOnlyList<SpecResponse>>> Handle(GetSpecsByClassIdQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<SpecResponse> characterSpecs = await dbContext.CharacterSpecs
            .Where(x => x.ClassId == query.ClassId)
            .Select(x => new SpecResponse(
                x.Id,
                x.Name,
                x.Type,
                x.Description,
                x.Stats.Select(s => s.ToDto()).ToArray(),
                x.ActionAssets.Select(a => a.ToDto()).ToArray()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(characterSpecs);
    }
}
