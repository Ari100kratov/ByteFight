using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Auth.Users;
using Domain.Game.Characters;
using Domain.Game.Characters.CharacterCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Game.Characters.CharacterCodes.Update;

internal class UpdateCodesCommandHandler(
    IGameDbContext dbContext,
    IUserAccessService userAccessService,
    IDateTimeProvider dateTimeProvider,
    ILogger<UpdateCodesCommandHandler> logger)
    : ICommandHandler<UpdateCodesCommand>
{
    public async Task<Result> Handle(UpdateCodesCommand command, CancellationToken cancellationToken)
    {
        Character? character = await dbContext.Characters
            .Include(c => c.Codes)
            .SingleOrDefaultAsync(c => c.Id == command.CharacterId, cancellationToken);

        if (character is null)
        {
            return Result.Failure(CharacterErrors.NotFound(command.CharacterId));
        }

        if (!await userAccessService.CanAccessUserOwnedResourceAsync(character.UserId.Value, cancellationToken))
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        foreach (Guid id in command.DeletedIds)
        {
            CharacterCode? code = character.Codes.FirstOrDefault(c => c.Id == id);

            if (code is null)
            {
                logger.LogWarning(
                    "Attempted to delete non-existent code: {CharacterCodeId} for CharacterId: {CharacterId}",
                    id,
                    command.CharacterId);
                continue;
            }

            dbContext.CharacterCodes.Remove(code);
        }

        foreach (CharacterCodeDto dto in command.Updated)
        {
            CharacterCode? code = character.Codes.FirstOrDefault(c => c.Id == dto.Id);

            if (code is null)
            {
                logger.LogWarning(
                    "Attempted to update non-existent code: {CharacterCodeId} for CharacterId: {CharacterId}",
                    dto.Id,
                    command.CharacterId);
                continue;
            }

            code.Name = dto.Name.Trim();
            code.SourceCode = dto.SourceCode;
            code.UpdatedAt = dateTimeProvider.UtcNow;
        }

        foreach (CharacterCodeDto dto in command.Created)
        {
            var newCode = new CharacterCode
            {
                Id = dto.Id,
                Name = dto.Name.Trim(),
                Language = CodeLanguage.CSharp,
                SourceCode = dto.SourceCode,
                CreatedAt = dateTimeProvider.UtcNow,
                CharacterId = character.Id
            };

            await dbContext.CharacterCodes.AddAsync(newCode, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
