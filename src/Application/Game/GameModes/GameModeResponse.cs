namespace Application.Game.GameModes;

public sealed class GameModeResponse
{
    public required int Id { get; init; }
    public required string Slug { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}
