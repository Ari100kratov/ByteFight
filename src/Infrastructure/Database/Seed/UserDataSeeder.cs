using Application.Abstractions.Authorization;
using Application.Abstractions.Messaging;
using Application.Auth.Users.Register;
using Domain.Auth.Roles;
using Domain.Auth.Users;
using Infrastructure.Database.Auth;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database.Seed;

public class UserDataSeeder(
    ICommandHandler<RegisterUserCommand, Guid> registerHandler,
    AuthDbContext dbContext)
{
    public async Task Seed(SeedContext seed, CancellationToken cancellationToken = default)
    {
        User? existingAdmin = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Email == "admin@bytefight.ru", cancellationToken);

        if (existingAdmin is null)
        {
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
        }
        else
        {
            seed.AdminId = existingAdmin.Id;
        }

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
