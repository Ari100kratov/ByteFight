using Domain.Game.ArenaItems;
using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs.Entries;

/// <summary>
/// Запись журнала о подборе предмета юнитом.
/// </summary>
public sealed class ItemPickedUpLogEntry : GameActionLogEntry
{
    /// <summary>
    /// Идентификатор размещённого предмета на арене.
    /// </summary>
    public Guid PlacedItemId { get; private set; }

    /// <summary>
    /// Идентификатор шаблона предмета.
    /// </summary>
    public Guid ItemId { get; private set; }

    /// <summary>
    /// Наименование предмета.
    /// </summary>
    public string ItemName { get; private set; }

    /// <summary>
    /// Тип предмета.
    /// </summary>
    public ArenaItemType ItemType { get; private set; }

    /// <summary>
    /// Позиция, на которой предмет был подобран.
    /// </summary>
    public Position Position { get; private set; }

    /// <summary>
    /// Значение применённого эффекта.
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// HP юнита после применения эффекта.
    /// </summary>
    public StatSnapshot ActorHp { get; private set; }

    private ItemPickedUpLogEntry() // EF
    {
    }

    public ItemPickedUpLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        Guid placedItemId,
        Guid itemId,
        string itemName,
        ArenaItemType itemType,
        Position position,
        decimal value,
        StatSnapshot actorHp,
        int turnIndex)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.ItemPickedUp,
            info,
            turnIndex)
    {
        PlacedItemId = placedItemId;
        ItemId = itemId;
        ItemName = itemName;
        ItemType = itemType;
        Position = position;
        Value = value;
        ActorHp = actorHp;
    }
}
