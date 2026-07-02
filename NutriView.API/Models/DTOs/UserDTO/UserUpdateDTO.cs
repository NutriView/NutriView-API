using NutriView.API.Helpers;

namespace NutriView.API.Models.DTOs
{
    public class UserUpdateDTO
    {
        public int DailyCalorieGoal { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public int? Age { get; set; }

        public GenderEnum? Gender { get; set; }

        public string? Image { get; set; }
    }
}