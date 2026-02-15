using System.Reflection;
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
            references: UserScriptCompilationReferences.Get(),
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release));

        IReadOnlyList<UserScriptSecurityIssue> securityIssues = UserScriptSecurityValidator.Validate(compilation, syntaxTree);
        if (securityIssues.Count > 0)
        {
            string message = string.Join('\n', securityIssues.Select(i => $"{i.Code}: {i.Message} (L{i.StartLine}:{i.StartColumn})"));
            throw new InvalidOperationException(message);
        }

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
            string errors = string.Join(
                "\n",
                result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString()));

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
}
