using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddFaqChatTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FAQNodes",
                columns: new[] { "NodeId", "IsFinalAnswer", "QuestionText" },
                values: new object[,]
                {
                    { 5, true, "Please go to the baggage services desk or file a lost baggage report at the arrivals hall." },
                    { 6, true, "Take photos and report the damage at the baggage service desk before leaving the airport." },
                    { 7, true, "Use your reference number to track the bag and contact baggage services if it does not arrive." }
                });

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 3,
                column: "Label",
                value: "Lost baggage");

            migrationBuilder.InsertData(
                table: "FAQOptions",
                columns: new[] { "OptionId", "BotMessageId", "Label", "NextNodeId", "ParentNodeId" },
                values: new object[,]
                {
                    { 4, null, "Damaged baggage", null, null },
                    { 5, null, "Delayed baggage", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FAQNodes",
                keyColumn: "NodeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FAQNodes",
                keyColumn: "NodeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FAQNodes",
                keyColumn: "NodeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "FAQOptions",
                keyColumn: "OptionId",
                keyValue: 3,
                column: "Label",
                value: "Lost Item");
        }
    }
}
