namespace NutriView.API.Models.DTOs
{
    public class ReminderResponseDTO
    {
        public Guid ReminderId { get; set; }

        public Guid UserId { get; set; }

        public int MealId { get; set; }
        public string MealName { get; set; } = string.Empty;

        public TimeSpan TimeOfDay { get; set; }

        public bool IsActive { get; set; }
    }
}
