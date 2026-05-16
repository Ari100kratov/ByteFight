using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Auth.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Auth.Users.ChangePassword;

internal sealed class ChangeUserPasswordCommandHandler(
    IAuthDbContext context,
    IPasswordHasher passwordHasher)
    : ICommandHandler<ChangeUserPasswordCommand>
{
    public async Task<Result> Handle(
        ChangeUserPasswordCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure(UserErrors.InvalidCurrentPassword);
        }

        user.PasswordHash = passwordHasher.Hash(command.NewPassword);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
