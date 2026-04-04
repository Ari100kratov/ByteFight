using System.Reflection;
using System.Text;
using System.Text.Json;
using GameRuntime.Logic.User.Api;
using GameRuntime.Logic.User.Api.Infrastructure;

namespace GameRuntime.UserCodeWorker;

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static int Main()
    {
        try
        {
            Console.InputEncoding = new UTF8Encoding(false);
            Console.OutputEncoding = new UTF8Encoding(false);

            string inputJson = Console.In.ReadToEnd();

            WorkerInput? input = JsonSerializer.Deserialize<WorkerInput>(inputJson, JsonOptions)
                ?? throw new InvalidOperationException("Некорректный input.");

            byte[] assemblyBytes = File.ReadAllBytes(input.AssemblyPath);
            var assembly = Assembly.Load(assemblyBytes);

            Type scriptType = assembly.GetType("UserScript")
                ?? throw new InvalidOperationException("Тип UserScript не найден.");

            MethodInfo decideMethod = scriptType.GetMethod(
                "Decide",
                BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException("Метод UserScript.Decide не найден.");

            var action = (UserAction?)decideMethod.Invoke(null, [input.World]);

            WorkerOutput output = new(
                Success: true,
                Action: action ?? throw new InvalidOperationException("Пользовательский код вернул null."),
                Error: null);

            Console.Out.Write(JsonSerializer.Serialize(output, JsonOptions));
            return 0;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            WorkerOutput output = new(
                Success: false,
                Action: null,
                Error: ex.InnerException.Message);

            Console.Out.Write(JsonSerializer.Serialize(output, JsonOptions));
            return 1;
        }
        catch (Exception ex)
        {
            WorkerOutput output = new(
                Success: false,
                Action: null,
                Error: ex.Message);

            Console.Out.Write(JsonSerializer.Serialize(output, JsonOptions));
            return 1;
        }
    }
}
