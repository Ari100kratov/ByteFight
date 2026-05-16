using Application.Abstractions.Authorization;
using Application.Auth.Users.Register;
using Domain.Auth.Roles;
using Infrastructure.Database.Auth;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Messaging;

namespace Infrastructure.Database.Seed;

public class UserDataSeeder(
    ICommandHandler<RegisterUserCommand, Guid> registerHandler,
    AuthDbContext dbContext)
{
    public async Task Seed(SeedContext seed, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        Result<Guid> result = await registerHandler.Handle(
            new RegisterUserCommand(
                Email: "admin@bytefight.ru",
                FirstName: "System",
                LastName: "Admin",
                Password: "admin123"
            ),
            cancellationToken);

        if (result.IsFailure)
        {
            throw new Exception($"Failed to seed admin user: {result.Error}");
        }

        seed.AdminId = result.Value;

        Role adminRole = await dbContext.Roles
            .SingleAsync(r => r.Name == Roles.Admin, cancellationToken);

        bool alreadyAssigned = await dbContext.UserRoles.AnyAsync(
            x => x.UserId == seed.AdminId && x.RoleId == adminRole.Id,
            cancellationToken);

        if (!alreadyAssigned)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = seed.AdminId,
                RoleId = adminRole.Id
            });

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
