using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class TakenFromAllowMinusInExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7bb2b653-b2cf-4d3a-8193-167cdfb0643b");

            migrationBuilder.AddColumn<decimal>(
                name: "TakenFromAllowedMinus",
                table: "Expenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c1255b71-480d-49c2-8a51-ab3fe2f1eb48", null, "user", "user" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c1255b71-480d-49c2-8a51-ab3fe2f1eb48");

            migrationBuilder.DropColumn(
                name: "TakenFromAllowedMinus",
                table: "Expenses");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7bb2b653-b2cf-4d3a-8193-167cdfb0643b", null, "user", "user" });
        }
    }
}
