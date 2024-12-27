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
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    content = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    occured_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    processed_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    error = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    concurrency_stamp = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true),
                    deleted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    personal_id = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    culture_info = table.Column<string>(type: "TEXT", nullable: false),
                    time_zone = table.Column<string>(type: "TEXT", nullable: false),
                    email_confirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    security_stamp = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    concurrency_stamp = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    access_failed_count = table.Column<int>(type: "INTEGER", nullable: false),
                    email_shadow_hash = table.Column<string>(type: "TEXT", nullable: false),
                    personal_id_shadow_hash = table.Column<string>(type: "TEXT", nullable: false),
                    phone_number_shadow_hash = table.Column<string>(type: "TEXT", nullable: false),
                    username_shadow_hash = table.Column<string>(type: "TEXT", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true),
                    deleted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    role_id = table.Column<string>(type: "TEXT", nullable: false),
                    claim_type = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    claim_value = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true),
                    deleted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "f_k_role_claim_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    user_id = table.Column<string>(type: "TEXT", nullable: false),
                    claim_type = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    claim_value = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true),
                    deleted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_claim_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    user_id = table.Column<string>(type: "TEXT", nullable: false),
                    user_agent = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    location = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ip_address = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    last_active = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    user_agent_shadow_hash = table.Column<string>(type: "TEXT", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true),
                    deleted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    deleted_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_login", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_login_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "TEXT", nullable: false),
                    role_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "f_k_user_role_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_user_role_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_role_name",
                table: "role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_role_claim_role_id",
                table: "role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "i_x_user_personal_id",
                table: "user",
                column: "personal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_user_claim_user_id",
                table: "user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_user_login_user_id",
                table: "user_login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_user_role_role_id",
                table: "user_role",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "role_claim");

            migrationBuilder.DropTable(
                name: "user_claim");

            migrationBuilder.DropTable(
                name: "user_login");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
