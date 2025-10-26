using Application.Abstractions.Messaging;
using Application.Auth.Users.Register;
using Infrastructure.Database.Auth;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database.Seed;

public class AuthDataSeeder(
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
                Password: "Ari100krat"
            ),
            cancellationToken
        );

        if (result.IsFailure)
        {
            throw new Exception($"Failed to seed admin user: {result.Error}");
        }

        seed.AdminId = result.Value;
    }
}
