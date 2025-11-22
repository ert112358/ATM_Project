using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATM_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class Reorganize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserName",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserName",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserName",
                table: "Transactions",
                column: "UserName");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserName",
                table: "Transactions",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
