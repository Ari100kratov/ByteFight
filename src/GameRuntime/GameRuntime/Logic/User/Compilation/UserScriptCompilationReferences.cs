using System.Collections.Immutable;
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

        typeof(Domain.ValueObjects.Position).Assembly.GetName().Name!,
        typeof(Domain.Game.Stats.StatType).Assembly.GetName().Name!,
        typeof(Api.UserWorldView).Assembly.GetName().Name!
    ];

    public static ImmutableArray<MetadataReference> Get()
    {
        string[] trustedAssemblies =
            ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        return [.. trustedAssemblies
            .Where(IsAllowed)
            .Select(CreateReferenceWithDocumentation)];
    }

    private static bool IsAllowed(string path)
    {
        string name = Path.GetFileNameWithoutExtension(path);
        return AllowedAssemblies.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    private static MetadataReference CreateReferenceWithDocumentation(string assemblyPath)
    {
        string xmlPath = Path.ChangeExtension(assemblyPath, ".xml");

        DocumentationProvider provider = File.Exists(xmlPath)
            ? XmlDocumentationProvider.CreateFromFile(xmlPath)
            : DocumentationProvider.Default;

        return MetadataReference.CreateFromFile(
            assemblyPath,
            documentation: provider);
    }
}
