using NutriView.API.Helpers;

namespace NutriView.API.Services
{
    public interface INutritionService
    {
        float CalculateCalories(float protein, float carbs, float fat, float fiber, float alcohol);

        /// <summary>
        /// Scales a food's per-measurement-base calories to the calories for a given quantity.
        /// </summary>
        float CalculateEntryCalories(float baseCalories, MeasurementBaseEnum measurementBase, float quantity);
    }
}