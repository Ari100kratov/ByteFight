using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Auth.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "auth");

        migrationBuilder.CreateTable(
            name: "roles",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                password_hash = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "role_permissions",
            schema: "auth",
            columns: table => new
            {
                role_id = table.Column<Guid>(type: "uuid", nullable: false),
                permission = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_role_permissions", x => new { x.role_id, x.permission });
                table.ForeignKey(
                    name: "fk_role_permissions_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "auth",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "refresh_tokens",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_revoked = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_refresh_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_refresh_tokens_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_roles",
            schema: "auth",
            columns: table => new
            {
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                role_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                table.ForeignKey(
                    name: "fk_user_roles_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "auth",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_roles_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_refresh_tokens_token",
            schema: "auth",
            table: "refresh_tokens",
            column: "token",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_refresh_tokens_user_id",
            schema: "auth",
            table: "refresh_tokens",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_roles_name",
            schema: "auth",
            table: "roles",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_role_id",
            schema: "auth",
            table: "user_roles",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            schema: "auth",
            table: "users",
            column: "email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "refresh_tokens",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "role_permissions",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "user_roles",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "roles",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "users",
            schema: "auth");
    }
}
