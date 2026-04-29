using System.ComponentModel.DataAnnotations;

namespace NutriView.API.Models.DTOs
{
    public class FoodEntryUpdateDTO
    {
        [Required]
        public float Quantity { get; set; }

        public int MealId { get; set; }

        public DateTime EntryDate { get; set; }
    }
}