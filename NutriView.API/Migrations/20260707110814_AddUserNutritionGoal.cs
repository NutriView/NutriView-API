using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriView.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNutritionGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NutritionValues_FoodId",
                table: "NutritionValues");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoodId",
                table: "NutritionValues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionValues_FoodId",
                table: "NutritionValues",
                column: "FoodId",
                unique: true,
                filter: "[FoodId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NutritionValues_FoodId",
                table: "NutritionValues");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoodId",
                table: "NutritionValues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionValues_FoodId",
                table: "NutritionValues",
                column: "FoodId",
                unique: true);
        }
    }
}
