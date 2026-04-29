namespace NutriView.API.Models.DTOs
{
    public class FoodEntryResponseDTO
    {
        public Guid FoodEntryId { get; set; }

        public Guid FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;

        public int MealId { get; set; }
        public string MealName { get; set; } = string.Empty;

        public float Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;

        public float Calories { get; set; }

        public DateTime EntryDate { get; set; }
    }
}