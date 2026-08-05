using NutriView.API.Helpers;

namespace NutriView.API.Services
{
    public class NutritionService : INutritionService
    {
        public float CalculateCalories(float protein, float carbs, float fat, float fiber, float alcohol)
        {
            var netCarbs = Math.Max(0, carbs - fiber/2);

            return (protein * 4) +
                   (netCarbs * 4) +
                   (fat * 9) +
                   (alcohol * 7);
        }

        public float CalculateEntryCalories(float baseCalories, MeasurementBaseEnum measurementBase, float quantity)
        {
            // Grams per one "base" unit. Weight-based units convert to grams;
            // count-based units (serving/cup/spoon) treat quantity as a count.
            var baseAmount = measurementBase switch
            {
                MeasurementBaseEnum.Per1g => 1f,
                MeasurementBaseEnum.Per100g => 100f,
                MeasurementBaseEnum.Per1kg => 1000f,
                MeasurementBaseEnum.Per1oz => 28.3495f,
                MeasurementBaseEnum.Per1lb => 453.592f,
                _ => 1f,
            };

            return baseCalories * (quantity / baseAmount);
        }
    }
}