namespace NutriView.API.Models.DTOs
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public int DailyCalorieGoal { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public int? Age { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}