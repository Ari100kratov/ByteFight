namespace Application.Game.CharacterCodes.GetTemplate;

public sealed record CodeTemplateResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string? SourceCode { get; init; }
}
