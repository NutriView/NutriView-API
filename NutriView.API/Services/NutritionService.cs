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
    }
}