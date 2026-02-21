using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Domain.Auth.Users;
using Domain.Game.Arenas;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Arenas.Update;

internal sealed class UpdateArenaCommandHandler(
    IGameDbContext dbContext,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateArenaCommand>
{
    public async Task<Result> Handle(UpdateArenaCommand command, CancellationToken cancellationToken)
    {
        Arena? arena = await dbContext.Arenas
            .SingleOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (arena is null)
        {
            return Result.Failure(ArenaErrors.NotFound(command.Id));
        }

        string name = command.Name.Trim();
        bool exists = await dbContext.Arenas.AnyAsync(a => a.Id != arena.Id && a.Name == name, cancellationToken);
        if (exists)
        {
            return Result.Failure(ArenaErrors.NameNotUnique);
        }

        if (arena.CreatedBy.Value != userContext.UserId)
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        arena.Name = name;
        arena.BackgroundAsset = command.BackgroundAsset?.Trim();
        arena.Description = command.Description?.Trim();
        arena.GameModes = command.GameModes;
        arena.IsActive = command.IsActive;
        arena.UpdatedAt = dateTimeProvider.UtcNow;

        arena.SetSize(command.GridWidth, command.GridHeight);
        arena.SetStartPosition(command.StartPosition?.ToValueObject() ?? new Position(0, 0));
        arena.SetBlockedPositions(command.BlockedPositions?.Select(x => x.ToValueObject()).ToArray() ?? []);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
