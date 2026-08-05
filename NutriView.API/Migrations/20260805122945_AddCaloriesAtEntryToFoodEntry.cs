using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriView.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCaloriesAtEntryToFoodEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CaloriesAtEntry",
                table: "FoodEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesAtEntry",
                table: "FoodEntries");
        }
    }
}
