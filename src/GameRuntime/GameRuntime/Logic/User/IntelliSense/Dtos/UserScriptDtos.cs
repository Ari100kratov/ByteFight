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
    IReadOnlyList<UserScriptSignatureDto> Signatures,
    int ActiveSignature,
    int ActiveParameter);

public sealed record UserScriptSignatureDto(
    string Signature,
    string? Documentation,
    IReadOnlyList<string> Parameters);
