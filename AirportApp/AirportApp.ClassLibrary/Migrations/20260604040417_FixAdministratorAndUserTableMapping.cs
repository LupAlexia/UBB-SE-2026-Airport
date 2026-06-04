using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class FixAdministratorAndUserTableMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_CreatorId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Senders_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Senders",
                principalColumn: "Sender_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Senders_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Senders",
                principalColumn: "Sender_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Senders_CreatorId",
                table: "Tickets",
                column: "CreatorId",
                principalTable: "Senders",
                principalColumn: "Sender_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Senders_UserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Senders_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Senders_CreatorId",
                table: "Tickets");

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Sender_Id);
                    table.ForeignKey(
                        name: "FK_Administrators_Senders_Sender_Id",
                        column: x => x.Sender_Id,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Sender_Id);
                    table.ForeignKey(
                        name: "FK_Users_Senders_Sender_Id",
                        column: x => x.Sender_Id,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                column: "Sender_Id",
                values: new object[]
                {
                    101,
                    102,
                    103
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Sender_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Sender_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_CreatorId",
                table: "Tickets",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Sender_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
