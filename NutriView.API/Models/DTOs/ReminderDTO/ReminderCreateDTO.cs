using System.ComponentModel.DataAnnotations;

namespace NutriView.API.Models.DTOs
{
    public class ReminderCreateDTO
    {
        [Required]
        public int MealId { get; set; }

        public TimeSpan TimeOfDay { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
