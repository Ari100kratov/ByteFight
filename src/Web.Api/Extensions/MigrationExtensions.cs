using Infrastructure.Database.Auth;
using Infrastructure.Database.Game;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        AuthMigrations(app);
        GameMigrations(app);
    }

    private static void AuthMigrations(IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using AuthDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        dbContext.Database.Migrate();
    }

    private static void GameMigrations(IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using GameDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<GameDbContext>();

        dbContext.Database.Migrate();
    }
}
