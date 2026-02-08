using GameRuntime.Logic.User.IntelliSense.Context;
using GameRuntime.Logic.User.IntelliSense.Model;
using GameRuntime.Logic.User.IntelliSense.Roslyn;
using Microsoft.CodeAnalysis;

namespace GameRuntime.Logic.User.IntelliSense;

public sealed class UserApiIntelliSenseProvider
{
    public ApiDescriptor Build()
    {
        Compilation compilation =
            new RoslynCompilationBuilder()
                .Build(ApiContextSource.Source);

        Dictionary<string, ApiTypeDescriptor> types =
            new RoslynTypeWalker()
                .Walk(compilation);

        return new ApiDescriptor
        {
            Globals = new()
            {
                ["world"] = "UserWorldView"
            },
            Types = types
        };
    }
}
