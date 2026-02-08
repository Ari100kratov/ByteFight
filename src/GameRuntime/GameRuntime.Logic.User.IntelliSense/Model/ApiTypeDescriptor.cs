namespace GameRuntime.Logic.User.IntelliSense.Model;

public sealed class ApiTypeDescriptor
{
    public required string Name { get; init; }
    public Dictionary<string, ApiPropertyDescriptor> Properties { get; init; } = [];
}
