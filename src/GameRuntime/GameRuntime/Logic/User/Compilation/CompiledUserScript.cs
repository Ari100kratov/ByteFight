namespace GameRuntime.Logic.User.Compilation;

public sealed class CompiledUserScript : IDisposable
{
    public string AssemblyName { get; }
    public byte[] AssemblyBytes { get; }

    /// <summary>
    /// Полный путь до временно сохраненной пользовательской сборки.
    /// Файл создается один раз и переиспользуется между ходами.
    /// </summary>
    public string AssemblyPath { get; }

    private readonly string _workDir;
    private bool _disposed;

    public CompiledUserScript(string assemblyName, byte[] assemblyBytes)
    {
        AssemblyName = assemblyName;
        AssemblyBytes = assemblyBytes;

        _workDir = Path.Combine(
            Path.GetTempPath(),
            "bytefight-user-scripts",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_workDir);

        AssemblyPath = Path.Combine(_workDir, $"{AssemblyName}.dll");
        File.WriteAllBytes(AssemblyPath, AssemblyBytes);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            if (Directory.Exists(_workDir))
            {
                Directory.Delete(_workDir, recursive: true);
            }
        }
        catch
        {
            // Пока непонятно, что в таком случае делать
        }
    }
}
