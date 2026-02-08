namespace GameRuntime.Logic.User.IntelliSense.Context;

internal static class ApiContextSource
{
    public const string Source = @"
using System;
using System.Linq;
using System.Collections.Generic;

using GameRuntime.Logic.User.Api;

public static class ScriptContext
{
    public static UserWorldView world;
}
";
}
