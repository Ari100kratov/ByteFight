using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Compilation;

internal static class UserScriptTemplate
{
    private const string UserCodeBegin = "//__USER_CODE_BEGIN__";
    private const string UserCodeEnd = "//__USER_CODE_END__";

    private const string Prefix = $@"
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
        {UserCodeBegin}
";

    private const string Suffix = $@"
        {UserCodeEnd}
    }}
}}
";

    private static readonly int UserCodeStartLine = Prefix.Split('\n').Length - 1;

    public static string Build(string userCode)
    {
        return Prefix + userCode + Suffix;
    }

    public static bool TryMapGeneratedSpanToUser(LinePositionSpan generatedSpan, out MappedLinePositionSpan mapped)
    {
        int startLine = generatedSpan.Start.Line - UserCodeStartLine;
        int endLine = generatedSpan.End.Line - UserCodeStartLine;

        if (startLine < 0 || endLine < 0)
        {
            mapped = default;
            return false;
        }

        mapped = new MappedLinePositionSpan(startLine, generatedSpan.Start.Character, endLine, generatedSpan.End.Character);
        return true;
    }

    public static void MapUserPositionToGenerated(int userLine, int userColumn, out int generatedLine, out int generatedColumn)
    {
        generatedLine = Math.Max(userLine, 0) + UserCodeStartLine;
        generatedColumn = Math.Max(userColumn, 0);
    }

    public readonly record struct MappedLinePositionSpan(int StartLine, int StartColumn, int EndLine, int EndColumn);
}
