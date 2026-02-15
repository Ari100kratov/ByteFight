namespace GameRuntime.Logic.User.Intellisense.Dtos;

public sealed record UserScriptDiagnosticDto(
    string Code,
    string Message,
    string Severity,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn);

public sealed record UserScriptCompletionDto(
    string Label,
    string? Detail,
    string Kind,
    string? Documentation);

public sealed record UserScriptHoverDto(
    string Signature,
    string? Documentation,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn);

public sealed record UserScriptSignatureHelpDto(
    string Signature,
    string? Documentation,
    int ActiveParameter,
    IReadOnlyList<string> Parameters);
