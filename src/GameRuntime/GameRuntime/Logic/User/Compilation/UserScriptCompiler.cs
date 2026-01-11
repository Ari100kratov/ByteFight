using System.Reflection;
using Domain.Game.Stats;
using Domain.ValueObjects;
using GameRuntime.Logic.User.Api;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace GameRuntime.Logic.User.Compilation;

internal sealed class UserScriptCompiler
{
    public Func<UserWorldView, UserAction> Compile(string userCode)
    {
        string source = UserScriptTemplate.Build(userCode);

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create(
            assemblyName: $"UserScript_{Guid.NewGuid()}",
            syntaxTrees: [syntaxTree],
            references: GetReferences(),
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release
            )
        );

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
            string errors = string.Join(
                "\n",
                result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString())
            );

            throw new InvalidOperationException(errors);
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        MethodInfo method = assembly
            .GetType("UserScript")!
            .GetMethod("Decide", BindingFlags.Public | BindingFlags.Static)!;

        return world =>
            (UserAction)method.Invoke(null, [world])!;
    }

    private static IEnumerable<MetadataReference> GetReferences()
    {
        // Все базовые assemblies текущего runtime
        string[] trustedAssemblies =
            ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        // Белый список — КРИТИЧНО
        string[] allowed =
        [
            "System.Runtime",
            "System.Private.CoreLib",
            "System.Linq",
            "System.Collections",
            "System.Console",
            "netstandard",

        // твои доменные сборки
        typeof(Position).Assembly.GetName().Name!,
        typeof(StatType).Assembly.GetName().Name!,
        typeof(UserWorldView).Assembly.GetName().Name!
        ];

        return trustedAssemblies
            .Where(path =>
            {
                string name = Path.GetFileNameWithoutExtension(path);
                return allowed.Contains(name, StringComparer.OrdinalIgnoreCase);
            })
            .Select(path => MetadataReference.CreateFromFile(path));
    }
}
