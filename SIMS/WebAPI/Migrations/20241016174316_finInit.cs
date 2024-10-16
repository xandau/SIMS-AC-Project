using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class finInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOG_ENTRIES",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TIMESTAMP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LEVEL = table.Column<int>(type: "int", nullable: false),
                    MESSAGE = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOG_ENTRIES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_UUID = table.Column<string>(type: "VARCHAR(300)", nullable: true),
                    USERNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FIRSTNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LASTNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PASSWORD_HASH = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PASSWORD_SALT = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ROLE = table.Column<int>(type: "int", nullable: true),
                    BLOCKED = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TICKETS",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TITLE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    STATE = table.Column<int>(type: "int", nullable: false),
                    CREATION_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Severity = table.Column<byte>(type: "tinyint", nullable: false),
                    CVE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TIMESTAMP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATOR_ID = table.Column<long>(type: "bigint", nullable: false),
                    ASSIGNEDPERSON_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TICKETS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TICKETS_USERS_ASSIGNEDPERSON_ID",
                        column: x => x.ASSIGNEDPERSON_ID,
                        principalTable: "USERS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TICKETS_USERS_CREATOR_ID",
                        column: x => x.CREATOR_ID,
                        principalTable: "USERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TICKETS_ASSIGNEDPERSON_ID",
                table: "TICKETS",
                column: "ASSIGNEDPERSON_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TICKETS_CREATOR_ID",
                table: "TICKETS",
                column: "CREATOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USERS_USERNAME",
                table: "USERS",
                column: "USERNAME",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOG_ENTRIES");

            migrationBuilder.DropTable(
                name: "TICKETS");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
