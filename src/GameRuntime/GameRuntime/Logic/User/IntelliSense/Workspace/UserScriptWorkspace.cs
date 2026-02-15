using System.Collections.Immutable;
using GameRuntime.Logic.User.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Intellisense.Workspace;

public sealed class UserScriptWorkspace : IDisposable
{
    private readonly AdhocWorkspace _workspace;
    private readonly DocumentId _documentId;

    private bool _disposed;

    private static readonly ImmutableArray<MetadataReference> References =
        UserScriptCompilationReferences.Get();

    private static readonly CSharpParseOptions ParseOptions =
        new(languageVersion: LanguageVersion.Latest, documentationMode: DocumentationMode.Diagnose);

    private static readonly CSharpCompilationOptions CompilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithAllowUnsafe(false)
            .WithOptimizationLevel(OptimizationLevel.Debug);

    public UserScriptWorkspace()
    {
        var host = MefHostServices.Create(
            MefHostServices.DefaultAssemblies);

        _workspace = new AdhocWorkspace(host);

        var _projectId = ProjectId.CreateNewId();
        _documentId = DocumentId.CreateNewId(_projectId);

        Solution solution = _workspace.CurrentSolution
            .AddProject(ProjectInfo.Create(
                _projectId,
                VersionStamp.Create(),
                "UserScriptProject",
                "UserScriptProject",
                LanguageNames.CSharp,
                parseOptions: ParseOptions,
                compilationOptions: CompilationOptions,
                metadataReferences: References))

            .AddDocument(
                _documentId,
                "UserScript.cs",
                SourceText.From(""));

        _workspace.TryApplyChanges(solution);
    }

    public Document UpdateDocument(string userCode)
    {
        ThrowIfDisposed();

        Solution solution = _workspace.CurrentSolution
            .WithDocumentText(_documentId, SourceText.From(UserScriptTemplate.Build(userCode)));

        _workspace.TryApplyChanges(solution);

        return _workspace.CurrentSolution.GetDocument(_documentId)!;
    }

    public Document GetDocument()
    {
        ThrowIfDisposed();

        return _workspace.CurrentSolution.GetDocument(_documentId)!;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(UserScriptWorkspace));
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _workspace.Dispose();
        _disposed = true;
    }
}
