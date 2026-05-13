using NutriView.API.Helpers;

namespace NutriView.API.Models.DTOs
{
    public class NutritionValueDTO
    {
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fat { get; set; }
        public float Sugar { get; set; }
        public float Fiber { get; set; }
        public float Sodium { get; set; }
        public float Alcohol { get; set; }

        public MeasurementBaseEnum MeasurementBase { get; set; } = MeasurementBaseEnum.Per100g;
    }
}