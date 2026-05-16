using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronicles.Infrastructure.Database.Migrations;

/// <inheritdoc />
[DbContext(typeof(ChroniclesDbContext))]
[Migration("20260515124000_AddNominationEntryParticipantMetadata")]
public partial class AddNominationEntryParticipantMetadata : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        AddColumns(migrationBuilder, "character_nomination_scores");
        AddColumns(migrationBuilder, "chronicle_records");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        DropColumns(migrationBuilder, "character_nomination_scores");
        DropColumns(migrationBuilder, "chronicle_records");
    }

    private static void AddColumns(MigrationBuilder migrationBuilder, string table)
    {
        migrationBuilder.AddColumn<string>(
            name: "user_first_name",
            schema: "chronicles",
            table: table,
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "user_last_name",
            schema: "chronicles",
            table: table,
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "character_class_name",
            schema: "chronicles",
            table: table,
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "character_spec_name",
            schema: "chronicles",
            table: table,
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);
    }

    private static void DropColumns(MigrationBuilder migrationBuilder, string table)
    {
        migrationBuilder.DropColumn(
            name: "user_first_name",
            schema: "chronicles",
            table: table);

        migrationBuilder.DropColumn(
            name: "user_last_name",
            schema: "chronicles",
            table: table);

        migrationBuilder.DropColumn(
            name: "character_class_name",
            schema: "chronicles",
            table: table);

        migrationBuilder.DropColumn(
            name: "character_spec_name",
            schema: "chronicles",
            table: table);
    }
}
