using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class FixFaqOptionLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 1,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 2,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { 4, 1 });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 3,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { 5, 2 });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 4,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { 6, 2 });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 5,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { 7, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 1,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 2,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 3,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 4,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 5,
                columns: new[] { "NextNodeId", "ParentNodeId" },
                values: new object[] { null, null });
        }
    }
}
