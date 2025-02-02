using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class SourcesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Account_AccountId",
                table: "Incomes");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e5ce00a-6d8b-4fa2-a84b-22c9b4c8a0cf");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Incomes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Incomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7bb2b653-b2cf-4d3a-8193-167cdfb0643b", null, "user", "user" });

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_SourceId",
                table: "Incomes",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SourceId",
                table: "Expenses",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Sources_SourceId",
                table: "Expenses",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Account_AccountId",
                table: "Incomes",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Sources_SourceId",
                table: "Incomes",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Sources_SourceId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Account_AccountId",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Sources_SourceId",
                table: "Incomes");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_SourceId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_SourceId",
                table: "Expenses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7bb2b653-b2cf-4d3a-8193-167cdfb0643b");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Incomes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Incomes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0e5ce00a-6d8b-4fa2-a84b-22c9b4c8a0cf", null, "user", "user" });

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Account_AccountId",
                table: "Incomes",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
