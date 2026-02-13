using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using GameRuntime.Logic.User.Api;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Compilation;

public sealed class UserScriptIntellisenseService : IDisposable
{
    private readonly AdhocWorkspace _workspace = new();

    public IReadOnlyList<UserScriptDiagnosticDto> GetDiagnostics(string userCode)
    {
        string source = UserScriptTemplate.Build(userCode);
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: $"UserScriptAnalysis_{Guid.NewGuid()}",
            syntaxTrees: [syntaxTree],
            references: UserScriptCompilationReferences.Get(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var roslynDiagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
            .Select(MapDiagnostic)
            .Where(x => x is not null)
            .Cast<UserScriptDiagnosticDto>();

        var securityDiagnostics = UserScriptSecurityValidator.Validate(compilation, syntaxTree)
            .Select(issue => new UserScriptDiagnosticDto(
                issue.Code,
                issue.Message,
                "Error",
                issue.StartLine,
                issue.StartColumn,
                issue.EndLine,
                issue.EndColumn));

        return [.. roslynDiagnostics, .. securityDiagnostics];
    }

    public async Task<IReadOnlyList<UserScriptCompletionDto>> GetCompletionsAsync(
        string userCode,
        int line,
        int column,
        CancellationToken cancellationToken)
    {
        Document? document = CreateDocument(userCode);
        if (document is null)
        {
            return [];
        }

        int sourcePosition = await MapToSourceOffsetAsync(document, line, column, cancellationToken);
        CompletionService? completionService = CompletionService.GetService(document);

        if (completionService is null)
        {
            return [];
        }

        CompletionList? completionList = await completionService.GetCompletionsAsync(document, sourcePosition, cancellationToken: cancellationToken);
        if (completionList is null)
        {
            return [];
        }

        var result = new List<UserScriptCompletionDto>();

        foreach (CompletionItem item in completionList.Items.Take(50))
        {
            CompletionDescription description = await completionService.GetDescriptionAsync(document, item, cancellationToken);
            result.Add(new UserScriptCompletionDto(
                item.DisplayText,
                item.InlineDescription,
                item.Tags.FirstOrDefault() ?? "text",
                ToPlainText(description.TaggedParts)));
        }

        return result;
    }

    public async Task<UserScriptHoverDto?> GetHoverAsync(string userCode, int line, int column, CancellationToken cancellationToken)
    {
        Document? document = CreateDocument(userCode);
        if (document is null)
        {
            return null;
        }

        SourceText sourceText = await document.GetTextAsync(cancellationToken);
        int sourcePosition = await MapToSourceOffsetAsync(document, line, column, cancellationToken);

        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);
        SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        if (root is null || semanticModel is null)
        {
            return null;
        }

        SyntaxToken token = root.FindToken(sourcePosition);
        if (token == default || token.Parent is null)
        {
            return null;
        }

        ISymbol? symbol = semanticModel.GetSymbolInfo(token.Parent, cancellationToken).Symbol
            ?? semanticModel.GetDeclaredSymbol(token.Parent, cancellationToken);

        if (symbol is null || IsForbiddenSymbol(symbol))
        {
            return null;
        }

        string signature = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        string? documentation = FormatXmlDocumentation(symbol.GetDocumentationCommentXml(cancellationToken: cancellationToken));

        LinePositionSpan lineSpan = sourceText.Lines.GetLinePositionSpan(token.Span);

        if (!UserScriptTemplate.TryMapGeneratedSpanToUser(lineSpan, out UserScriptTemplate.MappedLinePositionSpan mappedSpan))
        {
            return null;
        }

        return new UserScriptHoverDto(
            signature,
            documentation,
            mappedSpan.StartLine + 1,
            mappedSpan.StartColumn + 1,
            mappedSpan.EndLine + 1,
            mappedSpan.EndColumn + 1);
    }

    public async Task<UserScriptSignatureHelpDto?> GetSignatureHelpAsync(string userCode, int line, int column, CancellationToken cancellationToken)
    {
        Document? document = CreateDocument(userCode);
        if (document is null)
        {
            return null;
        }

        SourceText sourceText = await document.GetTextAsync(cancellationToken);
        int sourcePosition = await MapToSourceOffsetAsync(document, line, column, cancellationToken);
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);
        SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        if (root is null || semanticModel is null)
        {
            return null;
        }

        int tokenPosition = Math.Clamp(sourcePosition, 0, Math.Max(sourceText.Length - 1, 0));
        BaseArgumentListSyntax? argumentList = root
            .FindToken(tokenPosition)
            .Parent?
            .AncestorsAndSelf()
            .OfType<BaseArgumentListSyntax>()
            .FirstOrDefault(x => x.FullSpan.Contains(sourcePosition));

        if (argumentList is null)
        {
            return null;
        }

        int activeParameter = GetActiveParameterIndex(argumentList, sourcePosition);
        SyntaxNode? callableNode = argumentList.Parent switch
        {
            InvocationExpressionSyntax invocation => invocation.Expression,
            ObjectCreationExpressionSyntax creation => creation,
            ConstructorInitializerSyntax initializer => initializer,
            _ => null
        };

        if (callableNode is null)
        {
            return null;
        }

        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(callableNode, cancellationToken);
        var candidateMethods = GetCandidateMethods(symbolInfo)
            .Where(x => !IsForbiddenSymbol(x))
            .ToList();

        if (candidateMethods.Count == 0)
        {
            return null;
        }

        IMethodSymbol selectedMethod = candidateMethods
            .OrderByDescending(m => activeParameter >= 0 && activeParameter < m.Parameters.Length)
            .ThenBy(m => Math.Abs(m.Parameters.Length - (activeParameter + 1)))
            .First();

        var parameters = selectedMethod.Parameters
            .Select(p => p.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
            .ToList();

        string signature = selectedMethod.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        string? documentation = FormatXmlDocumentation(selectedMethod.GetDocumentationCommentXml(cancellationToken: cancellationToken));

        return new UserScriptSignatureHelpDto(signature, documentation, activeParameter, parameters);
    }

    private static bool IsForbiddenSymbol(ISymbol symbol)
    {
        var containingType = symbol switch
        {
            IMethodSymbol method => method.ContainingType,
            IPropertySymbol property => property.ContainingType,
            IFieldSymbol field => field.ContainingType,
            INamedTypeSymbol type => type,
            _ => null
        };

        if (containingType is null)
        {
            return false;
        }

        string fullName = containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", string.Empty, StringComparison.Ordinal);

        return fullName.StartsWith("System.IO.", StringComparison.Ordinal)
            || fullName.StartsWith("System.Net.", StringComparison.Ordinal)
            || fullName.StartsWith("System.Reflection.", StringComparison.Ordinal)
            || fullName.StartsWith("System.Diagnostics.", StringComparison.Ordinal)
            || fullName.StartsWith("System.Security.", StringComparison.Ordinal)
            || fullName.StartsWith("System.Runtime.InteropServices.", StringComparison.Ordinal)
            || fullName.StartsWith("System.Console", StringComparison.Ordinal);
    }

    private static UserScriptDiagnosticDto? MapDiagnostic(Diagnostic diagnostic)
    {
        FileLinePositionSpan lineSpan = diagnostic.Location.GetLineSpan();

        if (!UserScriptTemplate.TryMapGeneratedSpanToUser(
                new LinePositionSpan(lineSpan.StartLinePosition, lineSpan.EndLinePosition),
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

    private Document? CreateDocument(string userCode)
    {
        ProjectId projectId = ProjectId.CreateNewId();
        DocumentId documentId = DocumentId.CreateNewId(projectId);

        Solution solution = _workspace.CurrentSolution
            .AddProject(projectId, "UserScriptProject", "UserScriptProject", LanguageNames.CSharp)
            .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddMetadataReferences(projectId, UserScriptCompilationReferences.Get())
            .AddDocument(documentId, "UserScript.cs", SourceText.From(UserScriptTemplate.Build(userCode)));

        return solution.GetDocument(documentId);
    }

    private static IEnumerable<IMethodSymbol> GetCandidateMethods(SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol is IMethodSymbol method)
        {
            yield return method;
        }

        foreach (ISymbol candidate in symbolInfo.CandidateSymbols)
        {
            if (candidate is IMethodSymbol candidateMethod)
            {
                yield return candidateMethod;
            }
        }
    }

    private static int GetActiveParameterIndex(BaseArgumentListSyntax argumentList, int sourcePosition)
    {
        SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
        if (args.Count == 0)
        {
            return 0;
        }

        for (int i = 0; i < args.Count; i++)
        {
            if (sourcePosition <= args[i].Span.End)
            {
                return i;
            }
        }

        return Math.Max(args.Count - 1, 0);
    }

    private static async Task<int> MapToSourceOffsetAsync(Document document, int line, int column, CancellationToken cancellationToken)
    {
        SourceText text = await document.GetTextAsync(cancellationToken);
        UserScriptTemplate.MapUserPositionToGenerated(line - 1, column - 1, out int generatedLine, out int generatedColumn);

        if (generatedLine < 0 || generatedLine >= text.Lines.Count)
        {
            return text.Length;
        }

        TextLine textLine = text.Lines[generatedLine];
        int safeColumn = Math.Clamp(generatedColumn, 0, textLine.End - textLine.Start);
        return textLine.Start + safeColumn;
    }

    private static string ToPlainText(ImmutableArray<TaggedText> parts)
        => string.Concat(parts.Select(p => p.Text));

    private static string? FormatXmlDocumentation(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            return null;
        }

        string text = Regex.Replace(xml, "<.*?>", string.Empty);
        text = System.Net.WebUtility.HtmlDecode(text);
        text = Regex.Replace(text, "\\s+", " ").Trim();

        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    public void Dispose() => _workspace.Dispose();
}

public sealed record UserScriptDiagnosticDto(
    string Code,
    string Message,
    string Severity,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn);

public sealed record UserScriptCompletionDto(string Label, string? Detail, string Kind, string? Documentation);

public sealed record UserScriptHoverDto(string Signature, string? Documentation, int StartLine, int StartColumn, int EndLine, int EndColumn);

public sealed record UserScriptSignatureHelpDto(string Signature, string? Documentation, int ActiveParameter, IReadOnlyList<string> Parameters);
