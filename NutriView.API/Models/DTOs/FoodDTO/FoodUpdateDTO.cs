using System.ComponentModel.DataAnnotations;

namespace NutriView.API.Models.DTOs
{
    public class FoodUpdateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public bool IsGlobal { get; set; }

        [Required]
        public NutritionValueDTO Nutrition { get; set; } = new();
    }
}