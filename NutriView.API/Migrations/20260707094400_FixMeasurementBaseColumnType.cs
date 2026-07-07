using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriView.API.Migrations
{
    /// <inheritdoc />
    public partial class FixMeasurementBaseColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The ChangeMeasurementBaseToEnum migration was generated empty, so the
            // MeasurementBase column stayed nvarchar while the model maps it as an int
            // enum. Existing rows hold numeric strings (e.g. "0"), which EF can no longer
            // materialize -> InvalidCastException. Convert the column to int for real.
            migrationBuilder.Sql(
                "ALTER TABLE [NutritionValues] ALTER COLUMN [MeasurementBase] int NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE [NutritionValues] ALTER COLUMN [MeasurementBase] nvarchar(max) NOT NULL;");
        }
    }
}
