using System.Collections.Immutable;
using System.Reflection;
using GameRuntime.Logic.User.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Intellisense.Workspace;

public sealed class UserScriptRoslynContextFactory
{
    private readonly MefHostServices _host;

    private static readonly ImmutableArray<MetadataReference> References =
        UserScriptCompilationReferences.Get();

    private static readonly CSharpParseOptions ParseOptions =
        new(languageVersion: LanguageVersion.Latest, documentationMode: DocumentationMode.Diagnose);

    private static readonly CSharpCompilationOptions CompilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithAllowUnsafe(false)
            .WithOptimizationLevel(OptimizationLevel.Debug);

    public UserScriptRoslynContextFactory()
    {
        IEnumerable<Assembly> assemblies = MefHostServices.DefaultAssemblies
            .Concat(
            [
                typeof(CompletionService).Assembly,
                typeof(CSharpCompilation).Assembly,
                Assembly.Load("Microsoft.CodeAnalysis.Features"),
                Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features"),
                Assembly.Load("Microsoft.CodeAnalysis.CSharp.Workspaces"),
                Assembly.Load("Microsoft.CodeAnalysis.Workspaces"),
            ])
            .Distinct();

        _host = MefHostServices.Create(assemblies);
    }

    public UserScriptRoslynContext CreateContext(string userCode)
    {
        var workspace = new AdhocWorkspace(_host);

        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        Solution solution = workspace.CurrentSolution
            .AddProject(ProjectInfo.Create(
                projectId,
                VersionStamp.Create(),
                "UserScriptProject",
                "UserScriptProject",
                LanguageNames.CSharp,
                parseOptions: ParseOptions,
                compilationOptions: CompilationOptions,
                metadataReferences: References))
            .AddDocument(
                documentId,
                "UserScript.cs",
                SourceText.From(UserScriptTemplate.Build(userCode)));

        if (!workspace.TryApplyChanges(solution))
        {
            workspace.Dispose();
            throw new InvalidOperationException("Unable to apply Roslyn solution changes.");
        }

        Document document = workspace.CurrentSolution.GetDocument(documentId)
            ?? throw new InvalidOperationException("Roslyn document was not created.");

        return new UserScriptRoslynContext(workspace, document);
    }
}
