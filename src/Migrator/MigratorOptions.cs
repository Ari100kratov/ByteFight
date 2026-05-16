namespace Migrator;

internal sealed class MigratorOptions
{
    public const string SectionName = "Migrator";

    public int ChroniclesCatchUpBatchSize { get; init; } = 100;

    public bool RebuildChroniclesProjections { get; init; }
}
