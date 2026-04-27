namespace NutriView.API.Models.DTOs
{
    public class FoodResponseDTO
    {
        public Guid FoodId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public bool IsGlobal { get; set; }

        public NutritionValueDTO? Nutrition { get; set; }
    }
}