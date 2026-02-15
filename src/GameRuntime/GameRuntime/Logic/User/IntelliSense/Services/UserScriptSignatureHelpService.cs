using GameRuntime.Logic.User.Intellisense.Common;
using GameRuntime.Logic.User.Intellisense.Dtos;
using GameRuntime.Logic.User.Intellisense.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GameRuntime.Logic.User.Intellisense.Services;

public sealed class UserScriptSignatureHelpService
{
    private readonly UserScriptWorkspace _workspace;

    public UserScriptSignatureHelpService(UserScriptWorkspace workspace)
    {
        _workspace = workspace;
    }

    public async Task<UserScriptSignatureHelpDto?> GetSignatureHelpAsync(
        string userCode,
        int line,
        int column,
        CancellationToken ct)
    {
        Document document = _workspace.UpdateDocument(userCode);

        SyntaxNode? root = await document.GetSyntaxRootAsync(ct);
        SemanticModel? model = await document.GetSemanticModelAsync(ct);

        if (root is null || model is null)
        {
            return null;
        }

        int position = await RoslynMappingHelpers.MapToSourceOffsetAsync(document, line, column, ct);

        BaseArgumentListSyntax? argumentList = root.FindToken(position)
            .Parent?
            .AncestorsAndSelf()
            .OfType<BaseArgumentListSyntax>()
            .FirstOrDefault();

        if (argumentList is null)
        {
            return null;
        }

        if (argumentList.Parent is not InvocationExpressionSyntax invocation)
        {
            return null;
        }

        SymbolInfo symbolInfo = model.GetSymbolInfo(invocation.Expression, ct);

        IMethodSymbol? symbol =
            symbolInfo.Symbol as IMethodSymbol
            ?? symbolInfo.CandidateSymbols
                .OfType<IMethodSymbol>()
                .FirstOrDefault();

        if (symbol is null || RoslynSymbolHelpers.IsForbiddenSymbol(symbol))
        {
            return null;
        }

        int activeParam = argumentList.Arguments
            .TakeWhile(a => a.SpanStart < position)
            .Count();

        return new UserScriptSignatureHelpDto(
            symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            DocumentationFormatter.Format(symbol.GetDocumentationCommentXml(cancellationToken: ct)),
            activeParam,
            [.. symbol.Parameters.Select(p => p.ToDisplayString())]);
    }
}
