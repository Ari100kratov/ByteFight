using System.Collections.Immutable;
using System.Text;
using GameRuntime.Logic.User.Intellisense.Common;
using GameRuntime.Logic.User.Intellisense.Dtos;
using GameRuntime.Logic.User.Intellisense.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;

namespace GameRuntime.Logic.User.Intellisense.Services;

public sealed class UserScriptCompletionService
{
    private readonly UserScriptWorkspace _workspace;

    public UserScriptCompletionService(UserScriptWorkspace workspace)
    {
        _workspace = workspace;
    }

    public async Task<IReadOnlyList<UserScriptCompletionDto>> GetCompletions(
        string userCode,
        int line,
        int column,
        CancellationToken ct)
    {
        Document document = _workspace.UpdateDocument(userCode);
        int position = await RoslynMappingHelpers.MapToSourceOffsetAsync(document, line, column, ct);

        var completionService = CompletionService.GetService(document);
        if (completionService is null)
        {
            return [];
        }

        CompletionList? completionList = await completionService
            .GetCompletionsAsync(document, position, cancellationToken: ct);

        if (completionList is null)
        {
            return [];
        }

        CompletionItem[] items = [.. completionList.ItemsList.Take(50)];

        IEnumerable<Task<UserScriptCompletionDto>> tasks =
            items.Select(async item =>
            {
                CompletionDescription? description = await completionService
                    .GetDescriptionAsync(document, item, ct);

                string? documentation =
                    description is null
                        ? null
                        : FormatDocumentation(description.TaggedParts);

                return new UserScriptCompletionDto(
                    item.DisplayText,
                    item.InlineDescription,
                    GetKind(item.Tags),
                    documentation);
            });

        return await Task.WhenAll(tasks);
    }

    private static string GetKind(ImmutableArray<string> tags)
    {
        if (tags.IsDefaultOrEmpty)
        {
            return "text";
        }

        return tags[0];
    }

    private static string? FormatDocumentation(ImmutableArray<TaggedText> parts)
    {
        if (parts.IsDefaultOrEmpty)
        {
            return null;
        }

        var sb = new StringBuilder();

        foreach (TaggedText part in parts)
        {
            if (part.Tag is TextTags.LineBreak)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(part.Text);
            }
        }

        return sb.ToString().Trim();
    }
}
