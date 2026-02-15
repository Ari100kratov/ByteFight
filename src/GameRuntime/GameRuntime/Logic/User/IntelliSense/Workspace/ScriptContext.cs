using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Intellisense.Workspace;

public sealed class ScriptContext
{
    public required Document Document { get; init; }
    public required SourceText Text { get; init; }
    public required SyntaxNode Root { get; init; }
    public required SemanticModel SemanticModel { get; init; }
    public required int Position { get; init; }
}
