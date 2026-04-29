using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriView.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAlcoholToNutritionValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Alcohol",
                table: "NutritionValues",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alcohol",
                table: "NutritionValues");
        }
    }
}
