using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GameRuntime.UserCodeApiDocumentation;

/// <summary>
/// Собирает документацию пользовательского API один раз при старте приложения.
/// </summary>
public sealed class UserCodeApiDocHostedService(
    UserCodeApiDocGenerator generator,
    UserCodeApiDocSnapshot snapshot,
    IOptions<UserCodeApiDocOptions> options)
    : IHostedService
{
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        UserCodeApiDoc doc = generator.Generate(options.Value.Assemblies);
        snapshot.Set(doc);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
