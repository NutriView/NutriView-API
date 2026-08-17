using Microsoft.EntityFrameworkCore;
using NutriView.API.Data;
using NutriView.API.Exceptions;
using NutriView.API.Models.DTOs;
using NutriView.API.Models.Entities;

namespace NutriView.API.Services
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _context;
        private readonly INutritionService _nutritionService;

        public FoodService(ApplicationDbContext context, INutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        public async Task<IEnumerable<FoodResponseDTO>> GetAllAsync()
        {
            return await _context.Foods
                .Include(f => f.NutritionValue)
                .Select(f => new FoodResponseDTO
                {
                    FoodId = f.FoodId,
                    Name = f.Name,
                    Brand = f.Brand,
                    IsGlobal = f.IsGlobal,
                    Nutrition = f.NutritionValue == null ? null : new NutritionValueDTO
                    {
                        Calories = f.NutritionValue.Calories,
                        Protein = f.NutritionValue.Protein,
                        Carbs = f.NutritionValue.Carbs,
                        Fat = f.NutritionValue.Fat,
                        Sugar = f.NutritionValue.Sugar,
                        Fiber = f.NutritionValue.Fiber,
                        Sodium = f.NutritionValue.Sodium,
                        MeasurementBase = f.NutritionValue.MeasurementBase
                    }
                })
                .ToListAsync();
        }

        public async Task<FoodResponseDTO?> GetByIdAsync(Guid id)
        {
            var food = await _context.Foods
                .Include(f => f.NutritionValue)
                .FirstOrDefaultAsync(f => f.FoodId == id);

            if (food == null) return null;

            return new FoodResponseDTO
            {
                FoodId = food.FoodId,
                Name = food.Name,
                Brand = food.Brand,
                IsGlobal = food.IsGlobal,
                Nutrition = food.NutritionValue == null ? null : new NutritionValueDTO
                {
                    Calories = food.NutritionValue.Calories,
                    Protein = food.NutritionValue.Protein,
                    Carbs = food.NutritionValue.Carbs,
                    Fat = food.NutritionValue.Fat,
                    Sugar = food.NutritionValue.Sugar,
                    Fiber = food.NutritionValue.Fiber,
                    Sodium = food.NutritionValue.Sodium,
                    MeasurementBase = food.NutritionValue.MeasurementBase
                }
            };
        }

        public async Task<FoodResponseDTO> CreateAsync(Guid userId, FoodCreateDTO dto)
        {
            // Prevent duplicate foods in the shared catalog (same name + brand).
            if (dto.IsGlobal)
            {
                var duplicate = await _context.Foods.AnyAsync(f =>
                    f.IsGlobal &&
                    f.Name == dto.Name &&
                    f.Brand == dto.Brand);

                if (duplicate)
                    throw new ValidationException("A global food with this name and brand already exists");
            }

            var calories = _nutritionService.CalculateCalories(
                dto.Nutrition.Protein,
                dto.Nutrition.Carbs,
                dto.Nutrition.Fat,
                dto.Nutrition.Fiber,
                dto.Nutrition.Alcohol);

            var food = new Food
            {
                FoodId = Guid.NewGuid(),
                Name = dto.Name,
                Brand = dto.Brand,
                IsGlobal = dto.IsGlobal,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                NutritionValue = new NutritionValue
                {
                    NutritionValueId = Guid.NewGuid(),
                    Calories = calories,
                    Protein = dto.Nutrition.Protein,
                    Carbs = dto.Nutrition.Carbs,
                    Fat = dto.Nutrition.Fat,
                    Sugar = dto.Nutrition.Sugar,
                    Fiber = dto.Nutrition.Fiber,
                    Sodium = dto.Nutrition.Sodium,
                    MeasurementBase = dto.Nutrition.MeasurementBase
                }
            };

            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(food.FoodId)
                   ?? throw new Exception("Error creating food");
        }

        public async Task<bool> UpdateAsync(Guid id, FoodUpdateDTO dto)
        {
            var food = await _context.Foods
                .Include(f => f.NutritionValue)
                .FirstOrDefaultAsync(f => f.FoodId == id);

            if (food == null) return false;

            // Prevent renaming a global food onto another existing global food (same name + brand).
            if (dto.IsGlobal)
            {
                var duplicate = await _context.Foods.AnyAsync(f =>
                    f.FoodId != id &&
                    f.IsGlobal &&
                    f.Name == dto.Name &&
                    f.Brand == dto.Brand);

                if (duplicate)
                    throw new ValidationException("A global food with this name and brand already exists");
            }

            food.Name = dto.Name;
            food.Brand = dto.Brand;
            food.IsGlobal = dto.IsGlobal;
            var calories = _nutritionService.CalculateCalories(
               dto.Nutrition.Protein,
               dto.Nutrition.Carbs,
               dto.Nutrition.Fat,
               dto.Nutrition.Fiber,
               dto.Nutrition.Alcohol);

            if (food.NutritionValue != null)
            {
                food.NutritionValue.Calories = calories;
                food.NutritionValue.Protein = dto.Nutrition.Protein;
                food.NutritionValue.Carbs = dto.Nutrition.Carbs;
                food.NutritionValue.Fat = dto.Nutrition.Fat;
                food.NutritionValue.Sugar = dto.Nutrition.Sugar;
                food.NutritionValue.Fiber = dto.Nutrition.Fiber;
                food.NutritionValue.Sodium = dto.Nutrition.Sodium;
                food.NutritionValue.MeasurementBase = dto.Nutrition.MeasurementBase;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) return false;

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}