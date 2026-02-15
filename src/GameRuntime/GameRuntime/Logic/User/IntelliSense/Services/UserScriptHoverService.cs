using GameRuntime.Logic.User.Intellisense.Common;
using GameRuntime.Logic.User.Intellisense.Dtos;
using GameRuntime.Logic.User.Intellisense.Workspace;
using Microsoft.CodeAnalysis;

namespace GameRuntime.Logic.User.Intellisense.Services;

public sealed class UserScriptHoverService
{
    private readonly UserScriptWorkspace _workspace;

    public UserScriptHoverService(UserScriptWorkspace workspace)
    {
        _workspace = workspace;
    }

    public async Task<UserScriptHoverDto?> GetHoverAsync(
        string userCode,
        int line,
        int column,
        CancellationToken ct)
    {
        Document document = _workspace.UpdateDocument(userCode);

        SyntaxNode? root = await document.GetSyntaxRootAsync(ct);
        SemanticModel? semanticModel = await document.GetSemanticModelAsync(ct);

        if (root is null || semanticModel is null)
        {
            return null;
        }

        int position = await RoslynMappingHelpers.MapToSourceOffsetAsync(document, line, column, ct);

        SyntaxToken token = root.FindToken(position);
        if (token.Parent is null)
        {
            return null;
        }

        ISymbol? symbol = semanticModel.GetSymbolInfo(token.Parent, ct).Symbol
            ?? semanticModel.GetDeclaredSymbol(token.Parent, ct);

        if (symbol is null || RoslynSymbolHelpers.IsForbiddenSymbol(symbol))
        {
            return null;
        }

        string signature = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        string? documentation = DocumentationFormatter.Format(symbol.GetDocumentationCommentXml(cancellationToken: ct));

        return new UserScriptHoverDto(
            signature,
            documentation,
            line,
            column,
            line,
            column);
    }
}
