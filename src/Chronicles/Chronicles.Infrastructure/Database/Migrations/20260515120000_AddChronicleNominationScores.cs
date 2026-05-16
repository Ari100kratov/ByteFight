using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronicles.Infrastructure.Database.Migrations;

/// <inheritdoc />
[DbContext(typeof(ChroniclesDbContext))]
[Migration("20260515120000_AddChronicleNominationScores")]
public partial class AddChronicleNominationScores : Migration
{
    private static readonly string[] ScoreNominationCharacterColumns = ["nomination_type", "character_id"];
    private static readonly string[] ScoreNominationValueColumns = ["nomination_type", "value"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "metric_label",
            schema: "chronicles",
            table: "chronicle_nominations",
            type: "character varying(64)",
            maxLength: 64,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "metric_unit",
            schema: "chronicles",
            table: "chronicle_nominations",
            type: "character varying(32)",
            maxLength: 32,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "sort_direction",
            schema: "chronicles",
            table: "chronicle_nominations",
            type: "integer",
            nullable: false,
            defaultValue: 2);

        migrationBuilder.AddColumn<int>(
            name: "value_kind",
            schema: "chronicles",
            table: "chronicle_nominations",
            type: "integer",
            nullable: false,
            defaultValue: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "value",
            schema: "chronicles",
            table: "chronicle_records",
            type: "numeric(18,4)",
            precision: 18,
            scale: 4,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.CreateTable(
            name: "character_nomination_scores",
            schema: "chronicles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                character_id = table.Column<Guid>(type: "uuid", nullable: false),
                character_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                nomination_type = table.Column<int>(type: "integer", nullable: false),
                value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: true),
                occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_nomination_scores", x => x.id);
            });

        migrationBuilder.Sql("""
            INSERT INTO chronicles.chronicle_nominations
                (id, type, code, title, description, metric_label, metric_unit, value_kind, sort_direction)
            VALUES
                ('5ff8b1ae-41ec-4d95-8fd6-2e0d7d9bfa01', 1, 'arena_speedrun', 'Спидран по арене', 'Закончил бой раньше, чем зрители успели выбрать фаворита.', 'Быстрее всех', 'ходов', 1, 1),
                ('d9a1e3de-62b8-48d3-b317-5e9a1236f902', 2, 'battle_telenovela', 'Война на истощение', 'К концу боя у всех появились личные счеты.', 'Дольше всех', 'ходов', 1, 2),
                ('0fa30d9b-6fbd-4946-bf68-aeb9e0e85a03', 3, 'fight_subscription', 'Абонемент в драку', 'Этот персонаж приходит на арену чаще, чем домой.', 'Всего боев', 'боев', 2, 2),
                ('58cc6d97-33de-4391-a4ad-88c9f4dfa32f', 4, 'clockwork_victory', 'Победа по расписанию', 'Побеждает с такой стабильностью, будто исход уже известен.', 'Всего побед', 'побед', 2, 2),
                ('227512c1-2273-4319-a0f5-b4a92f2c9608', 5, 'useful_experience_master', 'Магистр полезного опыта', 'Не проигрывает, а собирает материал для ретроспективы.', 'Всего поражений', 'поражений', 2, 2),
                ('e6e7e1d1-3126-4975-a631-91dd43076460', 6, 'fist_diplomat', 'Дипломат с кулаками', 'Настолько любит ничьи, что враги уже готовы к переговорам.', 'Всего ничьих', 'ничьих', 2, 2),
                ('33b6fcaf-183d-40c6-b5bf-f088ecc8e1d3', 7, 'unlicensed_surgeon', 'Хирург без лицензии', 'Аккуратно объясняет врагам, где у них полоска здоровья.', 'Нанесено урона', 'урона', 3, 2),
                ('6399577b-713d-48f9-8604-583bb16fd66a', 8, 'budget_ambulance', 'Скорая помощь на минималках', 'Восстанавливает HP и мораль, но чек всё равно придёт.', 'Вылечено', 'лечения', 4, 2),
                ('f84b2877-ca0b-42f1-9f9e-c2c0c380351e', 9, 'armored_cardio', 'Кардио в броне', 'Прошел по арене больше всех и доказал, что позиция решает не меньше удара.', 'Перемещений', 'шагов', 2, 2),
                ('1898ec4a-cae2-4b4a-b547-058d42c5507e', 10, 'arena_vacuum_cleaner', 'Пылесос арены', 'Если предмет лежал на полу, значит он уже почти в инвентаре.', 'Подобрано', 'предметов', 2, 2),
                ('1d1e5810-c05a-4522-8b62-14d53eea80b1', 11, 'strategic_idle', 'Философский простой', 'Пропускал ходы так уверенно, что это почти стратегия.', 'Пропусков', 'пропусков', 2, 2),
                ('5e07c03d-748a-4a09-82e1-e5dff489ac98', 12, 'damage_sponge', 'Губка для урона', 'Впитывает удары лучше, чем документация впитывает TODO.', 'Получено урона', 'урона', 3, 2)
            ON CONFLICT (type) DO UPDATE SET
                code = EXCLUDED.code,
                title = EXCLUDED.title,
                description = EXCLUDED.description,
                metric_label = EXCLUDED.metric_label,
                metric_unit = EXCLUDED.metric_unit,
                value_kind = EXCLUDED.value_kind,
                sort_direction = EXCLUDED.sort_direction;
            """);

        migrationBuilder.CreateIndex(
            name: "ix_character_nomination_scores_nomination_type_character_id",
            schema: "chronicles",
            table: "character_nomination_scores",
            columns: ScoreNominationCharacterColumns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_character_nomination_scores_nomination_type_value",
            schema: "chronicles",
            table: "character_nomination_scores",
            columns: ScoreNominationValueColumns);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM chronicles.chronicle_nominations
            WHERE type IN (4, 5, 6, 7, 8, 9, 10, 11, 12);

            UPDATE chronicles.chronicle_nominations
            SET code = 'кратчайшая_победа',
                title = 'Кратчайшая победа',
                description = 'Самая быстрая победа по количеству ходов.'
            WHERE type = 1;

            UPDATE chronicles.chronicle_nominations
            SET code = 'самая_долгая_битва',
                title = 'Самая долгая битва',
                description = 'Битва с наибольшим количеством ходов.'
            WHERE type = 2;

            UPDATE chronicles.chronicle_nominations
            SET code = 'больше_всего_битв',
                title = 'Больше всего битв',
                description = 'Самый активный персонаж за выбранный период.'
            WHERE type = 3;
            """);

        migrationBuilder.DropTable(
            name: "character_nomination_scores",
            schema: "chronicles");

        migrationBuilder.AlterColumn<int>(
            name: "value",
            schema: "chronicles",
            table: "chronicle_records",
            type: "integer",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,4)",
            oldPrecision: 18,
            oldScale: 4);

        migrationBuilder.DropColumn(
            name: "metric_label",
            schema: "chronicles",
            table: "chronicle_nominations");

        migrationBuilder.DropColumn(
            name: "metric_unit",
            schema: "chronicles",
            table: "chronicle_nominations");

        migrationBuilder.DropColumn(
            name: "sort_direction",
            schema: "chronicles",
            table: "chronicle_nominations");

        migrationBuilder.DropColumn(
            name: "value_kind",
            schema: "chronicles",
            table: "chronicle_nominations");
    }
}
