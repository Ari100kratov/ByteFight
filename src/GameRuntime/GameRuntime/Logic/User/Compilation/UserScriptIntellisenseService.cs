using System.Collections.Immutable;
using System.Text.RegularExpressions;
using GameRuntime.Logic.User.Api;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.SignatureHelp;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Compilation;

public sealed class UserScriptIntellisenseService
{
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
        if (token == default)
        {
            return null;
        }

        ISymbol? symbol = semanticModel.GetSymbolInfo(token.Parent!, cancellationToken).Symbol
            ?? semanticModel.GetDeclaredSymbol(token.Parent!, cancellationToken);

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

        int sourcePosition = await MapToSourceOffsetAsync(document, line, column, cancellationToken);
        SignatureHelpService? service = SignatureHelpService.GetService(document);

        if (service is null)
        {
            return null;
        }

        SignatureHelpItems? items = await service.GetItemsAsync(document, sourcePosition, cancellationToken: cancellationToken);
        if (items is null || items.Items.Length == 0)
        {
            return null;
        }

        SignatureHelpItem selected = items.Items[Math.Clamp(items.SelectedItemIndex, 0, items.Items.Length - 1)];

        string prefix = ToPlainText(selected.PrefixDisplayParts);
        string separator = ToPlainText(selected.SeparatorDisplayParts);
        string suffix = ToPlainText(selected.SuffixDisplayParts);

        var parameters = selected.Parameters
            .Select(p => ToPlainText(p.DisplayParts))
            .ToList();

        string signature = prefix + string.Join(separator, parameters) + suffix;
        string? documentation = FormatXmlDocumentation(ToPlainText(selected.DocumentationFactory(cancellationToken)));

        return new UserScriptSignatureHelpDto(signature, documentation, items.ArgumentIndex, parameters);
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
            diagnostic.GetMessage(),
            diagnostic.Severity.ToString(),
            mapped.StartLine + 1,
            mapped.StartColumn + 1,
            mapped.EndLine + 1,
            mapped.EndColumn + 1);
    }

    private static Document? CreateDocument(string userCode)
    {
        var workspace = new AdhocWorkspace();
        ProjectId projectId = ProjectId.CreateNewId();
        DocumentId documentId = DocumentId.CreateNewId(projectId);

        Solution solution = workspace.CurrentSolution
            .AddProject(projectId, "UserScriptProject", "UserScriptProject", LanguageNames.CSharp)
            .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddMetadataReferences(projectId, UserScriptCompilationReferences.Get())
            .AddDocument(documentId, "UserScript.cs", SourceText.From(UserScriptTemplate.Build(userCode)));

        return solution.GetDocument(documentId);
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
