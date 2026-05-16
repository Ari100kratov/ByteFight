using Application.Abstractions.Data;

using Domain.Auth.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Application.Auth.Users.UpdateProfile;

internal sealed class UpdateUserProfileCommandHandler(
    IAuthDbContext context)
    : ICommandHandler<UpdateUserProfileCommand>
{
    public async Task<Result> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        bool emailTaken = await context.Users.AnyAsync(
            u => u.Email == command.Email && u.Id != command.UserId,
            cancellationToken);

        if (emailTaken)
        {
            return Result.Failure(UserErrors.EmailNotUnique);
        }

        user.Email = command.Email.Trim();
        user.FirstName = command.FirstName.Trim();
        user.LastName = command.LastName.Trim();

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
