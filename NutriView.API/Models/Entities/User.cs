using System;
using System.Collections.Generic;
using NutriView.API.Helpers;

namespace NutriView.API.Models.Entities
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int DailyCalorieGoal { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public int? Age { get; set; }

        public GenderEnum? Gender { get; set; }

        public string? Image { get; set; }

        public Guid? NutritionDailyGoalId { get; set; }

        public ICollection<FoodEntry> FoodEntries { get; set; } = new List<FoodEntry>();

        public ICollection<UploadedImage> UploadedImages { get; set; } = new List<UploadedImage>();

        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

        public NutritionValue? NutritionDailyGoal { get; set; }
    }
}
