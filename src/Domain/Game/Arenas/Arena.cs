using Domain.Game.GameModes;
using SharedKernel;

namespace Domain.Game.Arenas;

public sealed class Arena : Entity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int GridWidth { get; set; }

    public int GridHeight { get; set; }

    public string? BackgroundAsset { get; set; }

    public string? Description { get; set; }

    public List<GameModeType> GameModes { get; set; } = [];

    public bool IsActive { get; set; }

    public UserId CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
