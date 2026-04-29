namespace NutriView.API.Models.DTOs
{
    public class UserUpdateDTO
    {
        public int DailyCalorieGoal { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public int? Age { get; set; }
    }
}