using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaloryTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToFoodLogsAndDailyGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FoodLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "DailyGoals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FoodLogs_UserId",
                table: "FoodLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyGoals_UserId",
                table: "DailyGoals",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyGoals_User_UserId",
                table: "DailyGoals",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodLogs_User_UserId",
                table: "FoodLogs",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyGoals_User_UserId",
                table: "DailyGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodLogs_User_UserId",
                table: "FoodLogs");

            migrationBuilder.DropIndex(
                name: "IX_FoodLogs_UserId",
                table: "FoodLogs");

            migrationBuilder.DropIndex(
                name: "IX_DailyGoals_UserId",
                table: "DailyGoals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FoodLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DailyGoals");
        }
    }
}
