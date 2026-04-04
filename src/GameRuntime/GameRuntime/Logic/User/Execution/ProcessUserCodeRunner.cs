using System.Diagnostics;
using System.Text;
using System.Text.Json;
using GameRuntime.Logic.User.Api;
using GameRuntime.Logic.User.Api.Infrastructure;
using GameRuntime.Logic.User.Compilation;
using Microsoft.Extensions.Logging;

namespace GameRuntime.Logic.User.Execution;

internal sealed partial class ProcessUserCodeRunner : IUserCodeRunner
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly string _workerExePath;
    private readonly ILogger<ProcessUserCodeRunner> _logger;

    public ProcessUserCodeRunner(
        string workerExePath,
        ILogger<ProcessUserCodeRunner> logger)
    {
        _workerExePath = workerExePath;
        _logger = logger;
    }

    public UserAction Run(
        CompiledUserScript script,
        UserWorldView world,
        TimeSpan timeout)
    {
        var input = new WorkerInput(
            AssemblyPath: script.AssemblyPath,
            World: world);

        string inputJson = JsonSerializer.Serialize(input, JsonOptions);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _workerExePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardInputEncoding = new UTF8Encoding(false),
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false)
            }
        };

        try
        {
            process.Start();

            Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
            Task<string> stderrTask = process.StandardError.ReadToEndAsync();

            process.StandardInput.Write(inputJson);
            process.StandardInput.Close();

            if (!process.WaitForExit((int)timeout.TotalMilliseconds))
            {
                TryKillProcess(process);

                throw new TimeoutException(
                    $"Пользовательский код превысил лимит выполнения ({timeout.TotalMilliseconds} ms).");
            }

            Task.WaitAll(stdoutTask, stderrTask);

            string stdout = stdoutTask.Result;
            string stderr = stderrTask.Result;

            if (process.ExitCode != 0)
            {
                string errorText = TryExtractWorkerError(stdout, stderr)
                    ?? "Ошибка выполнения пользовательского кода.";

                throw new InvalidOperationException(errorText);
            }

            if (string.IsNullOrWhiteSpace(stdout))
            {
                throw new InvalidOperationException("Worker не вернул результат.");
            }

            WorkerOutput? result = JsonSerializer.Deserialize<WorkerOutput>(stdout, JsonOptions)
                ?? throw new InvalidOperationException("Не удалось разобрать ответ worker-процесса.");

            if (!result.Success)
            {
                throw new InvalidOperationException(result.Error);
            }

            return result.Action
                ?? throw new InvalidOperationException("Worker не вернул действие.");
        }
        catch (AggregateException ex) when (ex.InnerExceptions.Count == 1)
        {
            throw ex.InnerExceptions[0];
        }
    }

    private static string? TryExtractWorkerError(string stdout, string stderr)
    {
        if (!string.IsNullOrWhiteSpace(stdout))
        {
            try
            {
                WorkerOutput? output = JsonSerializer.Deserialize<WorkerOutput>(stdout, JsonOptions);
                if (!string.IsNullOrWhiteSpace(output?.Error))
                {
                    return output.Error;
                }
            }
            catch
            {
                // Игнорируем и пробуем stderr.
            }
        }

        if (!string.IsNullOrWhiteSpace(stderr))
        {
            return stderr.Trim();
        }

        return null;
    }

    private void TryKillProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Не удалось принудительно завершить worker-процесс пользовательского кода.");
        }
    }
}
