using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Game.ArenaItems;

/// <summary>
/// Предмет, который может быть размещён на аренах.
/// </summary>
public sealed class ArenaItem : Entity
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Наименование.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Тип предмета.
    /// </summary>
    public ArenaItemType Type { get; set; }

    /// <summary>
    /// Визуальное описание предмета.
    /// </summary>
    public SpriteAnimation Sprite { get; set; }

    /// <summary>
    /// Значение эффекта предмета.
    /// </summary>
    public int Value { get; private set; }

    public void SetValue(int value)
    {
        if (value <= 0)
        {
            throw new DomainException(
                "invalid_arena_item_value",
                $"Value ({value}) должен быть больше 0");
        }

        Value = value;
    }
}
