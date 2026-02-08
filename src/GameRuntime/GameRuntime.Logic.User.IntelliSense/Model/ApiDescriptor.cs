namespace GameRuntime.Logic.User.IntelliSense.Model;

public sealed class ApiDescriptor
{
    public required Dictionary<string, string> Globals { get; init; }
    public required Dictionary<string, ApiTypeDescriptor> Types { get; init; }
}
