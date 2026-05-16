namespace Chronicles.Infrastructure.Outbox;

internal sealed class OutboxImportCursor
{
    public string Name { get; set; } = string.Empty;

    public DateTime LastMessageCreatedAtUtc { get; set; }

    public Guid LastMessageId { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
