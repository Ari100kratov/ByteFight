using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Auth.Users;
using Domain.Game.Characters;
using Domain.Game.Characters.CharacterCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Game.Characters.CharacterCodes.Update;

internal class UpdateCodesCommandHandler(
    IGameDbContext dbContext,
    IUserContext userContext,
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

        if (character.UserId.Value != userContext.UserId)
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        // === Удаление ===
        foreach (Guid id in command.DeletedIds)
        {
            CharacterCode? code = character.Codes.FirstOrDefault(c => c.Id == id);
            if (code is null)
            {
                logger.LogWarning("Attempted to delete non-existent code: {CharacterCodeId} for CharacterId: {CharacterId}", id, command.CharacterId);
                continue;
            }

            dbContext.CharacterCodes.Remove(code);
        }

        // === Обновление ===
        foreach (CharacterCodeDto dto in command.Updated)
        {
            CharacterCode? code = character.Codes.FirstOrDefault(c => c.Id == dto.Id);
            if (code is null)
            {
                logger.LogWarning("Attempted to update non-existent code: {CharacterCodeId} for CharacterId: {CharacterId}", dto.Id, command.CharacterId);
                continue;
            }

            code.Name = dto.Name.Trim();
            code.SourceCode = dto.SourceCode;
            code.UpdatedAt = dateTimeProvider.UtcNow;
        }

        // === Добавление ===
        foreach (CharacterCodeDto dto in command.Created)
        {
            var newCode = new CharacterCode
            {
                Id = dto.Id,
                Name = dto.Name.Trim(),
                Language = CodeLanguage.CSharp,
                SourceCode = dto.SourceCode,
                CreatedAt = dateTimeProvider.UtcNow,
                CharacterId = character.Id,
            };

            await dbContext.CharacterCodes.AddAsync(newCode, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
