using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.GameRuntime.Migrations;

/// <inheritdoc />
[DbContext(typeof(GameRuntimeDbContext))]
[Migration("20260514120000_AddIntegrationOutboxAndParticipantNames")]
public partial class AddIntegrationOutboxAndParticipantNames : Migration
{
    private static readonly string[] AggregateIdTypeColumns = ["aggregate_id", "type"];
    private static readonly string[] TypeCreatedAtUtcIdColumns = ["type", "created_at_utc", "id"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "integration");

        migrationBuilder.AddColumn<string>(
            name: "unit_name",
            schema: "game_runtime",
            table: "game_session_participants",
            type: "character varying(128)",
            maxLength: 128,
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            schema: "integration",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                aggregate_id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                payload = table.Column<string>(type: "jsonb", nullable: false),
                occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                error = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_outbox_messages", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_outbox_messages_aggregate_id_type",
            schema: "integration",
            table: "outbox_messages",
            columns: AggregateIdTypeColumns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_outbox_messages_type_created_at_utc_id",
            schema: "integration",
            table: "outbox_messages",
            columns: TypeCreatedAtUtcIdColumns);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "outbox_messages",
            schema: "integration");

        migrationBuilder.DropColumn(
            name: "unit_name",
            schema: "game_runtime",
            table: "game_session_participants");
    }
}
