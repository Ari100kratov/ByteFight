using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronicles.Infrastructure.Database.Migrations;

/// <inheritdoc />
[DbContext(typeof(ChroniclesDbContext))]
[Migration("20260516150000_UpdateChronicleNominationCopy")]
public partial class UpdateChronicleNominationCopy : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE chronicles.chronicle_nominations
            SET code = 'fight_pass',
                title = 'Абонемент на драку'
            WHERE type = 3;

            UPDATE chronicles.chronicle_nominations
            SET code = 'downsizing_specialist',
                title = 'Специалист по сокращениям',
                description = 'Уменьшает показатели здоровья быстрее, чем HR — зарплатные ожидания.'
            WHERE type = 7;

            UPDATE chronicles.chronicle_nominations
            SET code = 'kleptomancer',
                title = 'Клептомант',
                description = 'Предметы исчезают рядом с ним при загадочных обстоятельствах.'
            WHERE type = 10;
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE chronicles.chronicle_nominations
            SET code = 'fight_subscription',
                title = 'Абонемент в драку'
            WHERE type = 3;

            UPDATE chronicles.chronicle_nominations
            SET code = 'unlicensed_surgeon',
                title = 'Хирург без лицензии',
                description = 'Аккуратно объясняет врагам, где у них полоска здоровья.'
            WHERE type = 7;

            UPDATE chronicles.chronicle_nominations
            SET code = 'arena_vacuum_cleaner',
                title = 'Пылесос арены',
                description = 'Если предмет лежал на полу, значит он уже почти в инвентаре.'
            WHERE type = 10;
            """);
    }
}
