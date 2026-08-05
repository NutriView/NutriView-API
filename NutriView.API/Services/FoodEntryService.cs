using Microsoft.EntityFrameworkCore;
using NutriView.API.Data;
using NutriView.API.Exceptions;
using NutriView.API.Models.DTOs;
using NutriView.API.Models.Entities;

namespace NutriView.API.Services
{
    public class FoodEntryService : IFoodEntryService
    {
        private readonly ApplicationDbContext _context;
        private readonly INutritionService _nutritionService;

        public FoodEntryService(ApplicationDbContext context, INutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        public async Task<IEnumerable<FoodEntryResponseDTO>> GetAllByUserAsync(Guid userId)
        {
            return await _context.FoodEntries
                .Include(fe => fe.Food)
                .Include(fe => fe.Meal)
                .Where(fe => fe.UserId == userId)
                .Select(fe => new FoodEntryResponseDTO
                {
                    FoodEntryId = fe.FoodEntryId,
                    FoodId = fe.FoodId,
                    FoodName = fe.Food.Name,
                    MealId = fe.MealId,
                    MealName = fe.Meal.Name,
                    Quantity = fe.Quantity,
                    Unit = fe.Unit,
                    Calories = fe.CaloriesAtEntry,
                    EntryDate = fe.EntryDate
                })
                .ToListAsync();
        }

        public async Task<FoodEntryResponseDTO?> GetByIdAsync(Guid id)
        {
            var entry = await _context.FoodEntries
                .Include(fe => fe.Food)
                .Include(fe => fe.Meal)
                .FirstOrDefaultAsync(fe => fe.FoodEntryId == id);

            if (entry == null) return null;

            return new FoodEntryResponseDTO
            {
                FoodEntryId = entry.FoodEntryId,
                FoodId = entry.FoodId,
                FoodName = entry.Food.Name,
                MealId = entry.MealId,
                MealName = entry.Meal.Name,
                Quantity = entry.Quantity,
                Unit = entry.Unit,
                Calories = entry.CaloriesAtEntry,
                EntryDate = entry.EntryDate
            };
        }

        public async Task<FoodEntryResponseDTO> CreateAsync(FoodEntryCreateDTO dto)
        {
            var food = await _context.Foods
                .Include(f => f.NutritionValue)
                .FirstOrDefaultAsync(f => f.FoodId == dto.FoodId);

            if (food == null || food.NutritionValue == null)
                throw new ValidationException("Food or nutrition data not found");

            var mealExists = await _context.Meals.AnyAsync(m => m.MealId == dto.MealId);
            if (!mealExists)
                throw new ValidationException("Meal not found");

            var entry = new FoodEntry
            {
                FoodEntryId = Guid.NewGuid(),
                UserId = dto.UserId,
                FoodId = dto.FoodId,
                MealId = dto.MealId,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                CaloriesAtEntry = _nutritionService.CalculateEntryCalories(
                    food.NutritionValue.Calories,
                    food.NutritionValue.MeasurementBase,
                    dto.Quantity),
                EntryDate = dto.EntryDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.FoodEntries.Add(entry);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entry.FoodEntryId)
                   ?? throw new Exception("Error creating entry");
        }

        public async Task<bool> UpdateAsync(Guid id, FoodEntryUpdateDTO dto)
        {
            var entry = await _context.FoodEntries
                .Include(fe => fe.Food)
                .ThenInclude(f => f.NutritionValue)
                .FirstOrDefaultAsync(fe => fe.FoodEntryId == id);

            if (entry == null) return false;

            var mealExists = await _context.Meals.AnyAsync(m => m.MealId == dto.MealId);
            if (!mealExists)
                throw new ValidationException("Meal not found");

            entry.Quantity = dto.Quantity;
            entry.MealId = dto.MealId;
            entry.EntryDate = dto.EntryDate;

            // Re-snapshot calories for the new quantity (food nutrition is included above).
            if (entry.Food?.NutritionValue != null)
            {
                entry.CaloriesAtEntry = _nutritionService.CalculateEntryCalories(
                    entry.Food.NutritionValue.Calories,
                    entry.Food.NutritionValue.MeasurementBase,
                    dto.Quantity);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entry = await _context.FoodEntries.FindAsync(id);
            if (entry == null) return false;

            _context.FoodEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}