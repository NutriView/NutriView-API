namespace NutriView.API.Models.DTOs
{
    public class ReminderUpdateDTO
    {
        public int MealId { get; set; }

        public TimeSpan TimeOfDay { get; set; }

        public bool IsActive { get; set; }
    }
}
