﻿using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Game.Arenas;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Game.Arenas.Create;

internal sealed class CreateArenaCommandHandler(
    IGameDbContext dbContext,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateArenaCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateArenaCommand command, CancellationToken cancellationToken)
    {
        string name = command.Name.Trim();

        bool exists = await dbContext.Arenas.AnyAsync(a => a.Name == name, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(ArenaErrors.NameNotUnique);
        }

        var arena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            GridWidth = command.GridWidth,
            GridHeight = command.GridHeight,
            BackgroundAsset = command.BackgroundAsset?.Trim(),
            Description = command.Description?.Trim(),
            GameModes = command.GameModes,
            IsActive = true,
            CreatedBy = new UserId(userContext.UserId),
            CreatedAt = dateTimeProvider.UtcNow
        };

        dbContext.Arenas.Add(arena);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(arena.Id);
    }
}
