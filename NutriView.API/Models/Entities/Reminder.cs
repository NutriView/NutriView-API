using System;

namespace NutriView.API.Models.Entities
{
    public class Reminder
    {
        public Guid ReminderId { get; set; }

        public Guid UserId { get; set; }

        public int MealId { get; set; }

        public TimeSpan TimeOfDay { get; set; }

        public bool IsActive { get; set; }

        public User User { get; set; } = null!;

        public Meal Meal { get; set; } = null!;
    }
}
