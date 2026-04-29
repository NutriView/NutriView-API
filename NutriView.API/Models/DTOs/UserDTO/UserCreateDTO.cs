using System.ComponentModel.DataAnnotations;

namespace NutriView.API.Models.DTOs
{
    public class UserCreateDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public int DailyCalorieGoal { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public int? Age { get; set; }
    }
}