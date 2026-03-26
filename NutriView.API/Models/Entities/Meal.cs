using System.Collections.Generic;

namespace NutriView.API.Models.Entities
{
    public class Meal
    {
        public int MealId { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<FoodEntry> FoodEntries { get; set; } = new List<FoodEntry>();

        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
