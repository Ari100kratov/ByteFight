using System.Globalization;
using GameRuntime.Logic.User.Compilation;
using GameRuntime.Logic.User.Intellisense.Dtos;
using GameRuntime.Logic.User.Intellisense.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Intellisense.Services;

public sealed class UserScriptDiagnosticsService
{
    private readonly UserScriptWorkspace _workspace;

    public UserScriptDiagnosticsService(UserScriptWorkspace workspace)
    {
        _workspace = workspace;
    }

    public async Task<IReadOnlyList<UserScriptDiagnosticDto>> GetDiagnostics(
        string userCode,
        CancellationToken ct)
    {
        Document document = _workspace.UpdateDocument(userCode);
        if (await document.Project.GetCompilationAsync(ct) is not CSharpCompilation compilation)
        {
            return [];
        }

        SyntaxTree? syntaxTree = await document.GetSyntaxTreeAsync(ct);
        if (syntaxTree is null)
        {
            return [];
        }

        IEnumerable<UserScriptDiagnosticDto> roslynDiagnostics =
            compilation.GetDiagnostics(ct)
                .Where(IsRelevant)
                .Select(MapDiagnostic)
                .Where(x => x != null)!;

        IEnumerable<UserScriptDiagnosticDto> securityDiagnostics =
            UserScriptSecurityValidator
                .Validate(compilation, syntaxTree)
                .Select(issue =>
                    new UserScriptDiagnosticDto(
                        issue.Code,
                        issue.Message,
                        "Error",
                        issue.StartLine,
                        issue.StartColumn,
                        issue.EndLine,
                        issue.EndColumn));

        return [.. roslynDiagnostics, .. securityDiagnostics];
    }

    private static bool IsRelevant(Diagnostic d)
    {
        return d.Severity is DiagnosticSeverity.Error || d.Severity is DiagnosticSeverity.Warning;
    }

    private static UserScriptDiagnosticDto? MapDiagnostic(
        Diagnostic diagnostic)
    {
        FileLinePositionSpan span = diagnostic.Location.GetLineSpan();

        if (!UserScriptTemplate.TryMapGeneratedSpanToUser(
            new LinePositionSpan(
                span.StartLinePosition,
                span.EndLinePosition),
                out UserScriptTemplate.MappedLinePositionSpan mapped))
        {
            return null;
        }

        return new UserScriptDiagnosticDto(
            diagnostic.Id,
            diagnostic.GetMessage(CultureInfo.InvariantCulture),
            diagnostic.Severity.ToString(),
            mapped.StartLine + 1,
            mapped.StartColumn + 1,
            mapped.EndLine + 1,
            mapped.EndColumn + 1);
    }
}
