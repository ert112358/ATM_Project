using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATM_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class FixTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserName",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserName",
                table: "Transactions",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserName",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserName",
                table: "Transactions",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "Name");
        }
    }
}
