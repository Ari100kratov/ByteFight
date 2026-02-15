using GameRuntime.Logic.User.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GameRuntime.Logic.User.Intellisense.Common;

public static class RoslynMappingHelpers
{
    public static async Task<int> MapToSourceOffsetAsync(
        Document document,
        int line,
        int column,
        CancellationToken ct)
    {
        SourceText text = await document.GetTextAsync(ct);

        UserScriptTemplate.MapUserPositionToGenerated(
            line - 1,
            column - 1,
            out int generatedLine,
            out int generatedColumn);

        if (generatedLine < 0 || generatedLine >= text.Lines.Count)
        {
            return text.Length;
        }

        TextLine textLine = text.Lines[generatedLine];
        int safeColumn = Math.Clamp(generatedColumn, 0, textLine.End - textLine.Start);

        return textLine.Start + safeColumn;
    }
}
