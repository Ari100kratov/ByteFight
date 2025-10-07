using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth.Users;
using Domain.Game.Arenas;
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
        bool exists = await dbContext.Arenas.AnyAsync(a => a.Name == name, cancellationToken);
        if (exists)
        {
            return Result.Failure(ArenaErrors.NameNotUnique);
        }

        if (arena.CreatedBy.Value != userContext.UserId)
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        arena.Name = name;
        arena.GridWidth = command.GridWidth;
        arena.GridHeight = command.GridHeight;
        arena.BackgroundAsset = command.BackgroundAsset?.Trim();
        arena.Description = command.Description?.Trim();
        arena.GameModes = command.GameModes;
        arena.IsActive = command.IsActive;
        arena.UpdatedAt = dateTimeProvider.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
