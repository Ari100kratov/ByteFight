using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronicles.Infrastructure.Database.Migrations;

/// <inheritdoc />
[DbContext(typeof(ChroniclesDbContext))]
[Migration("20260514121000_InitialChronicles")]
public partial class InitialChronicles : Migration
{
    private static readonly string[] ChronicleRecordNominationValueColumns = ["nomination_type", "value"];
    private static readonly string[] ChronicleRecordUniquenessColumns = ["session_id", "nomination_type", "character_id"];
    private static readonly string[] InboxProcessingQueueColumns =
    [
        "processed_at_utc",
        "dead_lettered_at_utc",
        "next_attempt_at_utc",
        "received_at_utc"
    ];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "chronicles");

        migrationBuilder.CreateTable(
            name: "chronicle_nominations",
            schema: "chronicles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_chronicle_nominations", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "character_chronicle_stats",
            schema: "chronicles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                character_id = table.Column<Guid>(type: "uuid", nullable: false),
                character_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                battles_played = table.Column<int>(type: "integer", nullable: false),
                victories = table.Column<int>(type: "integer", nullable: false),
                defeats = table.Column<int>(type: "integer", nullable: false),
                draws = table.Column<int>(type: "integer", nullable: false),
                total_turns_played = table.Column<int>(type: "integer", nullable: false),
                last_session_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_chronicle_stats", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "chronicle_records",
            schema: "chronicles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                character_id = table.Column<Guid>(type: "uuid", nullable: false),
                character_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false),
                nomination_type = table.Column<int>(type: "integer", nullable: false),
                value = table.Column<int>(type: "integer", nullable: false),
                occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_chronicle_records", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "import_cursors",
            schema: "chronicles",
            columns: table => new
            {
                name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                last_message_created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                last_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_import_cursors", x => x.name);
            });

        migrationBuilder.CreateTable(
            name: "inbox_messages",
            schema: "chronicles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                payload = table.Column<string>(type: "jsonb", nullable: false),
                received_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                attempt_count = table.Column<int>(type: "integer", nullable: false),
                next_attempt_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                dead_lettered_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                error = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_inbox_messages", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_character_chronicle_stats_character_id",
            schema: "chronicles",
            table: "character_chronicle_stats",
            column: "character_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_chronicle_nominations_type",
            schema: "chronicles",
            table: "chronicle_nominations",
            column: "type",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_chronicle_records_nomination_type_value",
            schema: "chronicles",
            table: "chronicle_records",
            columns: ChronicleRecordNominationValueColumns);

        migrationBuilder.CreateIndex(
            name: "ix_chronicle_records_session_id_nomination_type_character_id",
            schema: "chronicles",
            table: "chronicle_records",
            columns: ChronicleRecordUniquenessColumns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_inbox_messages_processing_queue",
            schema: "chronicles",
            table: "inbox_messages",
            columns: InboxProcessingQueueColumns);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "character_chronicle_stats",
            schema: "chronicles");

        migrationBuilder.DropTable(
            name: "chronicle_nominations",
            schema: "chronicles");

        migrationBuilder.DropTable(
            name: "chronicle_records",
            schema: "chronicles");

        migrationBuilder.DropTable(
            name: "import_cursors",
            schema: "chronicles");

        migrationBuilder.DropTable(
            name: "inbox_messages",
            schema: "chronicles");
    }
}
