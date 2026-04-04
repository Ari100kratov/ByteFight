using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace GameRuntime.Logic.User.Compilation;

internal sealed class UserScriptCompiler
{
    public CompiledUserScript Compile(string userCode)
    {
        string source = UserScriptTemplate.Build(userCode);
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        string assemblyName = $"UserScript_{Guid.NewGuid():N}";

        var compilation = CSharpCompilation.Create(
            assemblyName: assemblyName,
            syntaxTrees: [syntaxTree],
            references: UserScriptCompilationReferences.Get(),
            options: new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release));

        IReadOnlyList<UserScriptSecurityIssue> securityIssues =
            UserScriptSecurityValidator.Validate(compilation, syntaxTree);

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

        return new CompiledUserScript(
            assemblyName: assemblyName,
            assemblyBytes: ms.ToArray());
    }
}
