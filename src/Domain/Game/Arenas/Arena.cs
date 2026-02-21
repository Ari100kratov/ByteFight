using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.GameModes;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Game.Arenas;

public sealed class Arena : Entity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int GridWidth { get; private set; }

    public int GridHeight { get; private set; }

    public string? BackgroundAsset { get; set; }

    public string? Description { get; set; }

    public List<GameModeType> GameModes { get; set; } = [];

    public Position StartPosition { get; private set; }

    public List<Position> BlockedPositions { get; private set; } = [];

    public bool IsActive { get; set; }

    public UserId CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ArenaEnemy> Enemies { get; set; }

    public void SetSize(int width, int height)
    {
        if (width <= 0)
        {
            throw new DomainException("invalid_grid_width", $"GridWidth ({width}) должен быть больше 0");
        }

        if (height <= 0)
        {
            throw new DomainException("invalid_grid_height", $"GridHeight ({height}) должен быть больше 0");
        }

        ValidatePosition(StartPosition, "start_position_out_of_bounds", width, height);
        foreach (Position pos in BlockedPositions)
        {
            ValidatePosition(pos, "blocked_position_out_of_bounds", width, height);
        }

        GridWidth = width;
        GridHeight = height;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStartPosition(Position startPosition)
    {
        ValidatePosition(startPosition, "start_position_out_of_bounds", GridWidth, GridHeight);
        StartPosition = startPosition;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBlockedPositions(Position[] blockedPositions)
    {
        foreach (Position pos in blockedPositions)
        {
            ValidatePosition(pos, "blocked_position_out_of_bounds", GridWidth, GridHeight);
        }

        BlockedPositions = [.. blockedPositions];
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidatePosition(Position? pos, string code, int width, int height)
    {
        if (pos is null)
        {
            return;
        }

        if (pos.X < 0 || pos.X >= width)
        {
            throw new DomainException(code, $"Position.X ({pos.X}) выходит за границы арены {width}x{height}");
        }

        if (pos.Y < 0 || pos.Y >= height)
        {
            throw new DomainException(code, $"Position.Y ({pos.Y}) выходит за границы арены {width}x{height}");
        }
    }
}
