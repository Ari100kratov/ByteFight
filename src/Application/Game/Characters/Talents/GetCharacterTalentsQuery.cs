using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Game.Talents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.Talents.GetTalents;

/// <summary>
/// Возвращает дерево талантов класса персонажа с выбранными рангами
/// и числом доступных очков.
/// </summary>
public sealed record GetCharacterTalentsQuery(Guid CharacterId) : IQuery<CharacterTalentsResponse>;

public sealed record CharacterTalentsResponse(
    int Level,
    int TalentPointsTotal,
    int TalentPointsSpent,
    int TalentPointsAvailable,
    string ClassType,
    IReadOnlyList<TalentBranchResponse> Branches);

public sealed record TalentBranchResponse(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<TalentNodeResponse> Nodes);

public sealed record TalentNodeResponse(
    string Id,
    string Name,
    string Description,
    int Tier,
    int MaxRank,
    int CurrentRank,
    bool IsPassive,
    bool CanLearn);

internal sealed class GetCharacterTalentsQueryHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService)
    : IQueryHandler<GetCharacterTalentsQuery, CharacterTalentsResponse>
{
    public async Task<Result<CharacterTalentsResponse>> Handle(
        GetCharacterTalentsQuery query,
        CancellationToken cancellationToken)
    {
        Domain.Game.Characters.Character? character = await dbContext.Characters
            .AsNoTracking()
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Include(x => x.Talents)
            .SingleOrDefaultAsync(x => x.Id == query.CharacterId, cancellationToken);

        if (character is null)
        {
            return Result.Failure<CharacterTalentsResponse>(
                Domain.Game.Characters.CharacterErrors.NotFound(query.CharacterId));
        }

        if (!await userAccessService.CanAccessUserOwnedResourceAsync(character.UserId.Value, cancellationToken))
        {
            return Result.Failure<CharacterTalentsResponse>(
                Domain.Game.Characters.CharacterErrors.NotFound(query.CharacterId));
        }

        TalentTreeDefinition tree = TalentCatalog.GetTree(character.Spec.Class.Type);
        IReadOnlyDictionary<string, int> chosen = character.GetTalentRanks();

        int spent = TalentRules.TotalSpentRanks(chosen);
        int total = TalentRules.PointsForLevel(character.Level);

        var branches = tree.Branches
            .Select(branch => new TalentBranchResponse(
                branch.Id,
                branch.Name,
                branch.Description,
                [.. tree.GetBranchNodes(branch.Id).Select(node => new TalentNodeResponse(
                    node.Id,
                    node.Name,
                    node.Description,
                    node.Tier,
                    node.MaxRank,
                    chosen.GetValueOrDefault(node.Id),
                    node.NodeType == TalentNodeType.Passive,
                    TalentRules.CanLearn(tree, character.Level, chosen, node.Id).IsSuccess))]))
            .ToList();

        var response = new CharacterTalentsResponse(
            character.Level,
            total,
            spent,
            total - spent,
            character.Spec.Class.Type.ToString(),
            branches);

        return Result.Success(response);
    }
}
