using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using GameRuntime.Logic.User.Api;

namespace GameRuntime.Logic.User.IntelliSense.Roslyn;

internal sealed class RoslynCompilationBuilder
{
    public Compilation Build(string source)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

        return CSharpCompilation.Create(
            "UserScript.IntelliSense",
            [tree],
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
    }

    private static IEnumerable<MetadataReference> GetReferences()
    {
        string[] trusted =
            ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        string[] allowed = new[]
        {
            "System.Runtime",
            "System.Linq",
            "System.Collections",
            "netstandard",

            typeof(UserWorldView).Assembly.GetName().Name!
        };

        return trusted
            .Where(p =>
                allowed.Contains(
                    Path.GetFileNameWithoutExtension(p),
                    StringComparer.OrdinalIgnoreCase))
            .Select(p => MetadataReference.CreateFromFile(p));
    }
}
