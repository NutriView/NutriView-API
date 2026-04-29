namespace NutriView.API.Services
{
    public interface INutritionService
    {
        float CalculateCalories(float protein, float carbs, float fat, float fiber, float alcohol);
    }
}
