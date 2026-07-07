namespace NutriView.API.Models.Entities
{
    public class FoodEntry
    {
        public Guid FoodEntryId { get; set; }

        public Guid UserId { get; set; }

        public Guid FoodId { get; set; }

        public int MealId { get; set; }

        public float Quantity { get; set; }

        public string Unit { get; set; } = "g";

        public DateTime EntryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public Food Food { get; set; } = null!;

        public Meal Meal { get; set; } = null!;
    }
}