using System.ComponentModel.DataAnnotations;

namespace NutriView.API.Models.DTOs
{
    public class FoodEntryCreateDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid FoodId { get; set; }

        [Required]
        public int MealId { get; set; }

        [Required]
        public float Quantity { get; set; }

        public string Unit { get; set; } = "g";

        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    }
}