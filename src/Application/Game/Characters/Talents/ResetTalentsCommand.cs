using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.Talents.Reset;

/// <summary>
/// Полностью сбрасывает таланты персонажа (очки возвращаются).
/// </summary>
public sealed record ResetTalentsCommand(Guid CharacterId) : ICommand;

internal sealed class ResetTalentsCommandHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService)
    : ICommandHandler<ResetTalentsCommand>
{
    public async Task<Result> Handle(ResetTalentsCommand command, CancellationToken cancellationToken)
    {
        Character? character = await dbContext.Characters
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

        if (character.Talents is { Count: > 0 } talents)
        {
            dbContext.CharacterTalents.RemoveRange(talents);
            character.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
