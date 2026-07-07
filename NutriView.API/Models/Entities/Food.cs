namespace NutriView.API.Models.Entities
{
    public class Food
    {
        public Guid FoodId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public bool IsGlobal { get; set; }

        public DateTime CreatedAt { get; set; }

        public User? CreatedByUser { get; set; }

        public NutritionValue? NutritionValue { get; set; }

        public ICollection<FoodEntry> FoodEntries { get; set; } = new List<FoodEntry>();
    }
}