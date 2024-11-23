using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_message",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    content = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    occured_on_utc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    error = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_outbox_message", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    full_name = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    avatar_url = table.Column<string>(type: "TEXT", nullable: true),
                    role = table.Column<int>(type: "INTEGER", nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", nullable: false),
                    security_stamp = table.Column<string>(type: "TEXT", nullable: false),
                    created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    last_modified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true),
                    deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_message");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
