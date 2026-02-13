using System.Collections.Immutable;
using Domain.Game.Stats;
using Domain.ValueObjects;
using GameRuntime.Logic.User.Api;
using Microsoft.CodeAnalysis;

namespace GameRuntime.Logic.User.Compilation;

internal static class UserScriptCompilationReferences
{
    private static readonly string[] AllowedAssemblies =
    [
        "System.Runtime",
        "System.Private.CoreLib",
        "System.Linq",
        "System.Collections",
        "netstandard",
        typeof(Position).Assembly.GetName().Name!,
        typeof(StatType).Assembly.GetName().Name!,
        typeof(UserWorldView).Assembly.GetName().Name!
    ];

    public static ImmutableArray<MetadataReference> Get()
    {
        string[] trustedAssemblies =
            ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        return [.. trustedAssemblies
            .Where(path =>
            {
                string name = Path.GetFileNameWithoutExtension(path);
                return AllowedAssemblies.Contains(name, StringComparer.OrdinalIgnoreCase);
            })
            .Select(MetadataReference.CreateFromFile)];
    }
}
