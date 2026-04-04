using GameRuntime.Logic.User.Api;
using GameRuntime.Logic.User.Compilation;

namespace GameRuntime.Logic.User.Execution;

internal interface IUserCodeRunner
{
    UserAction Run(
        CompiledUserScript script,
        UserWorldView world,
        TimeSpan timeout);
}
