using NutriView.API.Helpers;

namespace NutriView.API.Models.Entities
{
    public class NutritionValue
    {
        public Guid NutritionValueId { get; set; }

        // Null when this nutrition value is a standalone user daily goal
        // rather than the macros of a specific food.
        public Guid? FoodId { get; set; }

        public float Calories { get; set; }

        public float Protein { get; set; }

        public float Carbs { get; set; }

        public float Fat { get; set; }

        public float Sugar { get; set; }

        public float Fiber { get; set; }

        public float Sodium { get; set; }
        public float Alcohol { get; set; } = 0;
        public MeasurementBaseEnum MeasurementBase { get; set; } = MeasurementBaseEnum.Per100g;

        public Food? Food { get; set; }
    }
}