using Microsoft.CodeAnalysis;

namespace GameRuntime.Logic.User.Intellisense.Workspace;

public sealed class UserScriptRoslynContext : IDisposable
{
    private readonly AdhocWorkspace _workspace;
    private bool _disposed;

    public Document Document { get; }

    public UserScriptRoslynContext(
        AdhocWorkspace workspace,
        Document document)
    {
        _workspace = workspace;
        Document = document;
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
