using GameRuntime.Logic.User.Compilation;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Game.Characters.Codes;

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
        app.MapPost("characters/codes/intellisense/diagnostics", (
            DiagnosticsRequest request,
            UserScriptIntellisenseService intellisense) =>
        {
            if (!TryNormalizeInput(request.SourceCode, 1, 1, out string sourceCode, out _, out _, out IResult? error))
            {
                return error!;
            }

            return Results.Ok(intellisense.GetDiagnostics(sourceCode));
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();

        app.MapPost("characters/codes/intellisense/completions", async (
            CompletionsRequest request,
            UserScriptIntellisenseService intellisense,
            CancellationToken cancellationToken) =>
        {
            int line = request.Position?.Line ?? 1;
            int column = request.Position?.Column ?? 1;

            if (!TryNormalizeInput(request.SourceCode, line, column, out string sourceCode, out int safeLine, out int safeColumn, out IResult? error))
            {
                return error!;
            }

            var completions = await intellisense.GetCompletionsAsync(
                sourceCode,
                safeLine,
                safeColumn,
                cancellationToken);

            return Results.Ok(completions);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();


        app.MapPost("characters/codes/intellisense/signature-help", async (
            SignatureHelpRequest request,
            UserScriptIntellisenseService intellisense,
            CancellationToken cancellationToken) =>
        {
            int line = request.Position?.Line ?? 1;
            int column = request.Position?.Column ?? 1;

            if (!TryNormalizeInput(request.SourceCode, line, column, out string sourceCode, out int safeLine, out int safeColumn, out IResult? error))
            {
                return error!;
            }

            var signatureHelp = await intellisense.GetSignatureHelpAsync(
                sourceCode,
                safeLine,
                safeColumn,
                cancellationToken);

            return signatureHelp is null ? Results.NoContent() : Results.Ok(signatureHelp);
        })
        .WithTags(Tags.CharacterCodes)
        .RequireAuthorization();
        app.MapPost("characters/codes/intellisense/hover", async (
            HoverRequest request,
            UserScriptIntellisenseService intellisense,
            CancellationToken cancellationToken) =>
        {
            int line = request.Position?.Line ?? 1;
            int column = request.Position?.Column ?? 1;

            if (!TryNormalizeInput(request.SourceCode, line, column, out string sourceCode, out int safeLine, out int safeColumn, out IResult? error))
            {
                return error!;
            }

            var hover = await intellisense.GetHoverAsync(
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
