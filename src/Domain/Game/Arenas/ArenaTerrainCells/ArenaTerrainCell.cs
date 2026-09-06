using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Game.Arenas.ArenaTerrainCells;

/// <summary>
/// Гекс арены с особым рельефом.
/// Гексы без записи считаются луговиной (<see cref="TerrainType.Meadow"/>).
/// </summary>
public sealed class ArenaTerrainCell : Entity
{
    /// <summary>
    /// Уникальный идентификатор гекса рельефа.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор арены.
    /// </summary>
    public Guid ArenaId { get; set; }

    /// <summary>
    /// Арена.
    /// </summary>
    public Arena Arena { get; set; } = null!;

    /// <summary>
    /// Позиция гекса на арене.
    /// </summary>
    public Position Position { get; set; }

    /// <summary>
    /// Тип рельефа гекса.
    /// </summary>
    public TerrainType Terrain { get; set; }
}
