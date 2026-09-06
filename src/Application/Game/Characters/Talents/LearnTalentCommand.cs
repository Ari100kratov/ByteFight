using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Game.Characters;
using Domain.Game.Characters.CharacterTalents;
using Domain.Game.Talents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.Talents.Learn;

/// <summary>
/// Изучает следующий ранг таланта персонажа.
/// </summary>
public sealed record LearnTalentCommand(Guid CharacterId, string TalentId) : ICommand;

internal sealed class LearnTalentCommandHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService)
    : ICommandHandler<LearnTalentCommand>
{
    public async Task<Result> Handle(LearnTalentCommand command, CancellationToken cancellationToken)
    {
        Character? character = await dbContext.Characters
            .Include(x => x.Spec)
                .ThenInclude(x => x.Class)
            .Include(x => x.Talents)
            .SingleOrDefaultAsync(x => x.Id == command.CharacterId, cancellationToken);

        if (character is null)
        {
            return Result.Failure(CharacterErrors.NotFound(command.CharacterId));
        }

        if (!await userAccessService.CanAccessUserOwnedResourceAsync(character.UserId.Value, cancellationToken))
        {
            return Result.Failure(CharacterErrors.NotFound(command.CharacterId));
        }

        TalentTreeDefinition tree = TalentCatalog.GetTree(character.Spec.Class.Type);
        IReadOnlyDictionary<string, int> chosen = character.GetTalentRanks();

        Result canLearn = TalentRules.CanLearn(tree, character.Level, chosen, command.TalentId);

        if (canLearn.IsFailure)
        {
            return Result.Failure(canLearn.Error);
        }

        CharacterTalent? existing = character.Talents?.FirstOrDefault(x => x.TalentId == command.TalentId);

        if (existing is null)
        {
            dbContext.CharacterTalents.Add(new CharacterTalent
            {
                Id = Guid.CreateVersion7(),
                CharacterId = character.Id,
                TalentId = command.TalentId,
                Rank = 1
            });
        }
        else
        {
            existing.Rank++;
        }

        character.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
