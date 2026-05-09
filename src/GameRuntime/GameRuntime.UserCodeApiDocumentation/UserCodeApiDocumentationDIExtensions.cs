using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GameRuntime.UserCodeApiDocumentation;

/// <summary>
/// Регистрация документации пользовательского API.
/// </summary>
public static class UserCodeApiDocumentationDIExtensions
{
    /// <summary>
    /// Добавляет генерацию и хранение документации пользовательского API.
    /// </summary>
    public static IServiceCollection AddUserCodeApiDocumentation(
        this IServiceCollection services,
        IReadOnlyList<Assembly> assemblies)
    {
        services.Configure<UserCodeApiDocOptions>(options =>
        {
            options.Assemblies = assemblies;
        });

        services.AddSingleton<UserCodeApiDocGenerator>();
        services.AddSingleton<UserCodeApiDocSnapshot>();
        services.AddHostedService<UserCodeApiDocHostedService>();

        return services;
    }
}
