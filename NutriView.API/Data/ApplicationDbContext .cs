using Microsoft.EntityFrameworkCore;
using NutriView.API.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NutriView.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Food> Foods { get; set; }

        public DbSet<NutritionValue> NutritionValues { get; set; }

        public DbSet<FoodEntry> FoodEntries { get; set; }

        public DbSet<UploadedImage> UploadedImages { get; set; }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // A NutritionValue is optionally tied to a Food: food macros have a
            // FoodId, while a user daily goal is a standalone NutritionValue with
            // FoodId == null. Deleting a food still cascades to its macros.
            modelBuilder.Entity<NutritionValue>()
                .HasOne(n => n.Food)
                .WithOne(f => f.NutritionValue)
                .HasForeignKey<NutritionValue>(n => n.FoodId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodEntry>()
                .HasOne(fe => fe.User)
                .WithMany(u => u.FoodEntries)
                .HasForeignKey(fe => fe.UserId);

            modelBuilder.Entity<FoodEntry>()
                .HasOne(fe => fe.Food)
                .WithMany(f => f.FoodEntries)
                .HasForeignKey(fe => fe.FoodId);

            modelBuilder.Entity<FoodEntry>()
                .HasOne(fe => fe.Meal)
                .WithMany(m => m.FoodEntries)
                .HasForeignKey(fe => fe.MealId);

            modelBuilder.Entity<Meal>().HasData(
                new Meal { MealId = 1, Name = "Breakfast" },
                new Meal { MealId = 2, Name = "Lunch" },
                new Meal { MealId = 3, Name = "Dinner" },
                new Meal { MealId = 4, Name = "Snack" });
        }
    }
}
