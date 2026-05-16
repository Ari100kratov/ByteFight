namespace Chronicles.Infrastructure.Source.GameRuntime;

internal sealed class OutboxMessageReadModel
{
    public Guid Id { get; set; }

    public Guid AggregateId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime OccurredAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public string? Error { get; set; }
}
