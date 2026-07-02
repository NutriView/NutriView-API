using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriView.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderImageAndNutritionDailyGoalToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NutritionDailyGoalId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NutritionDailyGoalId",
                table: "Users",
                column: "NutritionDailyGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_NutritionValues_NutritionDailyGoalId",
                table: "Users",
                column: "NutritionDailyGoalId",
                principalTable: "NutritionValues",
                principalColumn: "NutritionValueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_NutritionValues_NutritionDailyGoalId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_NutritionDailyGoalId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NutritionDailyGoalId",
                table: "Users");
        }
    }
}
