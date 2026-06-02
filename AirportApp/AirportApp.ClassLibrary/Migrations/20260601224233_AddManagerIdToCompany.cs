using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerIdToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Company_Id",
                keyValue: 1,
                column: "ManagerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Company_Id",
                keyValue: 2,
                column: "ManagerId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Company_Id",
                keyValue: 3,
                column: "ManagerId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Company_Id",
                keyValue: 4,
                column: "ManagerId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Company_Id",
                keyValue: 5,
                column: "ManagerId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ManagerId",
                table: "Companies",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Managers_ManagerId",
                table: "Companies",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "Manager_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Managers_ManagerId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_ManagerId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Companies");
        }
    }
}
