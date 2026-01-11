namespace GameRuntime.Logic.User.Compilation;

internal static class UserScriptTemplate
{
    public static string Build(string userCode)
    {
        return $@"
using System;
using System.Linq;
using System.Collections.Generic;

using Domain.ValueObjects;
using Domain.Game.Stats;

using GameRuntime.Logic.User.Api;

public static class UserScript
{{
    public static UserAction Decide(UserWorldView world)
    {{
        {userCode}
    }}
}}
";
    }
}
