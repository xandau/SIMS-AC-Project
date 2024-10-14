using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOG_ENTRIES",
                columns: table => new
                {
                    LOG_ENTRY_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TIMESTAMP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LEVEL = table.Column<int>(type: "int", nullable: false),
                    MESSAGE = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOG_ENTRIES", x => x.LOG_ENTRY_ID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_UUID = table.Column<string>(type: "VARCHAR(300)", nullable: false),
                    USERNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FIRSTNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LASTNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PASSWORD = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ROLE = table.Column<int>(type: "int", nullable: false),
                    BLOCKED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "TICKETS",
                columns: table => new
                {
                    TICKET_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TITLE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    STATE = table.Column<int>(type: "int", nullable: false),
                    CREATION_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATOR_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TICKETS", x => x.TICKET_ID);
                    table.ForeignKey(
                        name: "FK_TICKETS_USERS_CREATOR_ID",
                        column: x => x.CREATOR_ID,
                        principalTable: "USERS",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TICKETS_CREATOR_ID",
                table: "TICKETS",
                column: "CREATOR_ID");
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
