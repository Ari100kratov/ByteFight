using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Compilation;

internal static class UserScriptSecurityValidator
{
    private static readonly string[] ForbiddenTypePrefixes =
    [
        "System.IO.",
        "System.Net.",
        "System.Reflection.",
        "System.Diagnostics.",
        "System.Threading.",
        "System.Runtime.InteropServices.",
        "System.Security.",
        "System.Environment",
        "System.AppDomain",
        "System.Activator",
        "System.Type",
        "System.GC",
        "System.Console"
    ];

    public static IReadOnlyList<UserScriptSecurityIssue> Validate(CSharpCompilation compilation, SyntaxTree syntaxTree)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
        SyntaxNode root = syntaxTree.GetRoot();

        var issues = new List<UserScriptSecurityIssue>();

        foreach (SyntaxNode node in root.DescendantNodes())
        {
            if (!IsNodeWithinUserCode(node, syntaxTree))
            {
                continue;
            }

            if (node.IsKind(SyntaxKind.UnsafeStatement) || node.IsKind(SyntaxKind.PointerType))
            {
                issues.Add(CreateIssue("SEC001", "Unsafe code is not allowed.", node, syntaxTree));
                continue;
            }

            if (node is ObjectCreationExpressionSyntax objectCreation)
            {
                ITypeSymbol? createdType = semanticModel.GetTypeInfo(objectCreation).Type;
                if (IsForbidden(createdType))
                {
                    issues.Add(CreateIssue("SEC002", "Using this type is not allowed in user scripts.", objectCreation, syntaxTree));
                }
            }

            if (node is InvocationExpressionSyntax invocation)
            {
                IMethodSymbol? methodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (IsForbidden(methodSymbol?.ContainingType))
                {
                    issues.Add(CreateIssue("SEC003", "Calling this API is not allowed in user scripts.", invocation, syntaxTree));
                }
            }

            if (node is MemberAccessExpressionSyntax memberAccess)
            {
                ISymbol? symbol = semanticModel.GetSymbolInfo(memberAccess).Symbol;
                if (symbol is not null && IsForbidden(symbol.ContainingType))
                {
                    issues.Add(CreateIssue("SEC004", "Access to this member is not allowed in user scripts.", memberAccess, syntaxTree));
                }
            }
        }

        return issues;
    }

    private static bool IsForbidden(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        string fullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", string.Empty, StringComparison.Ordinal);

        return ForbiddenTypePrefixes.Any(prefix => fullName.StartsWith(prefix, StringComparison.Ordinal));
    }

    private static bool IsNodeWithinUserCode(SyntaxNode node, SyntaxTree syntaxTree)
    {
        FileLinePositionSpan span = syntaxTree.GetLineSpan(node.Span);
        return UserScriptTemplate.TryMapGeneratedSpanToUser(
            new LinePositionSpan(span.StartLinePosition, span.EndLinePosition),
            out _);
    }

    private static UserScriptSecurityIssue CreateIssue(string code, string message, SyntaxNode node, SyntaxTree syntaxTree)
    {
        FileLinePositionSpan span = syntaxTree.GetLineSpan(node.Span);
        UserScriptTemplate.TryMapGeneratedSpanToUser(
            new LinePositionSpan(span.StartLinePosition, span.EndLinePosition),
            out UserScriptTemplate.MappedLinePositionSpan mapped);

        return new UserScriptSecurityIssue(
            code,
            message,
            mapped.StartLine + 1,
            mapped.StartColumn + 1,
            mapped.EndLine + 1,
            mapped.EndColumn + 1);
    }
}

internal sealed record UserScriptSecurityIssue(
    string Code,
    string Message,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn);
