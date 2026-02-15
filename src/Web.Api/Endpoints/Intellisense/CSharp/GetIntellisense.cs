using GameRuntime.Logic.User.Intellisense.Dtos;
using GameRuntime.Logic.User.Intellisense.Services;

namespace Web.Api.Endpoints.Intellisense.CSharp;

internal sealed class GetIntellisense : IEndpoint
{
    private const int MaxSourceCodeLength = 20_000;

    public sealed record PositionRequest(int Line, int Column);

    public sealed record DiagnosticsRequest(string? SourceCode);

    public sealed record CompletionsRequest(string? SourceCode, PositionRequest? Position);

    public sealed record HoverRequest(string? SourceCode, PositionRequest? Position);

    public sealed record SignatureHelpRequest(string? SourceCode, PositionRequest? Position);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("intellisense/csharp/diagnostics", async (
            DiagnosticsRequest request,
            UserScriptDiagnosticsService intellisense,
            CancellationToken cancellationToken) =>
        {
            if (!TryNormalizeInput(request.SourceCode, 1, 1, out string sourceCode, out _, out _, out IResult? error))
            {
                return error!;
            }

            IReadOnlyList<UserScriptDiagnosticDto> diagnostics = await intellisense.GetDiagnostics(sourceCode, cancellationToken);

            return Results.Ok(diagnostics);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();

        app.MapPost("intellisense/csharp/completions", async (
            CompletionsRequest request,
            UserScriptCompletionService intellisense,
            CancellationToken cancellationToken) =>
        {
            int line = request.Position?.Line ?? 1;
            int column = request.Position?.Column ?? 1;

            if (!TryNormalizeInput(request.SourceCode, line, column, out string sourceCode, out int safeLine, out int safeColumn, out IResult? error))
            {
                return error!;
            }

            IReadOnlyList<UserScriptCompletionDto> completions = await intellisense.GetCompletions(
                sourceCode,
                safeLine,
                safeColumn,
                cancellationToken);

            return Results.Ok(completions);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();


        app.MapPost("intellisense/csharp/signature-help", async (
            SignatureHelpRequest request,
            UserScriptSignatureHelpService intellisense,
            CancellationToken cancellationToken) =>
        {
            int line = request.Position?.Line ?? 1;
            int column = request.Position?.Column ?? 1;

            if (!TryNormalizeInput(request.SourceCode, line, column, out string sourceCode, out int safeLine, out int safeColumn, out IResult? error))
            {
                return error!;
            }

            UserScriptSignatureHelpDto? signatureHelp = await intellisense.GetSignatureHelpAsync(
                sourceCode,
                safeLine,
                safeColumn,
                cancellationToken);

            return signatureHelp is null ? Results.NoContent() : Results.Ok(signatureHelp);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();

        app.MapPost("intellisense/csharp/hover", async (
            HoverRequest request,
            UserScriptHoverService intellisense,
            CancellationToken cancellationToken) =>
        {
            int line = request.Position?.Line ?? 1;
            int column = request.Position?.Column ?? 1;

            if (!TryNormalizeInput(request.SourceCode, line, column, out string sourceCode, out int safeLine, out int safeColumn, out IResult? error))
            {
                return error!;
            }

            UserScriptHoverDto? hover = await intellisense.GetHoverAsync(
                sourceCode,
                safeLine,
                safeColumn,
                cancellationToken);

            return hover is null ? Results.NoContent() : Results.Ok(hover);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();
    }

    private static bool TryNormalizeInput(
        string? sourceCode,
        int line,
        int column,
        out string safeSourceCode,
        out int safeLine,
        out int safeColumn,
        out IResult? error)
    {
        safeSourceCode = sourceCode ?? string.Empty;

        if (safeSourceCode.Length > MaxSourceCodeLength)
        {
            safeLine = 1;
            safeColumn = 1;
            error = Results.BadRequest(new { message = $"SourceCode is too large. Max length is {MaxSourceCodeLength}." });
            return false;
        }

        safeLine = Math.Clamp(line, 1, 10_000);
        safeColumn = Math.Clamp(column, 1, 10_000);

        error = null;
        return true;
    }
}
