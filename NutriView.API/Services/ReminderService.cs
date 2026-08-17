using Microsoft.EntityFrameworkCore;
using NutriView.API.Data;
using NutriView.API.Exceptions;
using NutriView.API.Models.DTOs;
using NutriView.API.Models.Entities;

namespace NutriView.API.Services
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;

        public ReminderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReminderResponseDTO>> GetAllByUserAsync(Guid userId)
        {
            return await _context.Reminders
                .Include(r => r.Meal)
                .Where(r => r.UserId == userId)
                .Select(r => new ReminderResponseDTO
                {
                    ReminderId = r.ReminderId,
                    UserId = r.UserId,
                    MealId = r.MealId,
                    MealName = r.Meal.Name,
                    TimeOfDay = r.TimeOfDay,
                    IsActive = r.IsActive
                })
                .ToListAsync();
        }

        public async Task<ReminderResponseDTO?> GetByIdAsync(Guid userId, Guid id)
        {
            // Scoping by UserId is the ownership check: another user's reminder is
            // indistinguishable from one that does not exist.
            var reminder = await _context.Reminders
                .Include(r => r.Meal)
                .FirstOrDefaultAsync(r => r.ReminderId == id && r.UserId == userId);

            if (reminder == null) return null;

            return new ReminderResponseDTO
            {
                ReminderId = reminder.ReminderId,
                UserId = reminder.UserId,
                MealId = reminder.MealId,
                MealName = reminder.Meal.Name,
                TimeOfDay = reminder.TimeOfDay,
                IsActive = reminder.IsActive
            };
        }

        public async Task<ReminderResponseDTO> CreateAsync(Guid userId, ReminderCreateDTO dto)
        {
            // A token can outlive the account it was issued for, so the user is still checked.
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
                throw new ValidationException("User not found");

            var mealExists = await _context.Meals.AnyAsync(m => m.MealId == dto.MealId);
            if (!mealExists)
                throw new ValidationException("Meal not found");

            var reminder = new Reminder
            {
                ReminderId = Guid.NewGuid(),
                UserId = userId,
                MealId = dto.MealId,
                TimeOfDay = dto.TimeOfDay,
                IsActive = dto.IsActive
            };

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(userId, reminder.ReminderId)
                   ?? throw new Exception("Error creating reminder");
        }

        public async Task<bool> UpdateAsync(Guid userId, Guid id, ReminderUpdateDTO dto)
        {
            var reminder = await _context.Reminders
                .FirstOrDefaultAsync(r => r.ReminderId == id && r.UserId == userId);

            if (reminder == null) return false;

            var mealExists = await _context.Meals.AnyAsync(m => m.MealId == dto.MealId);
            if (!mealExists)
                throw new ValidationException("Meal not found");

            reminder.MealId = dto.MealId;
            reminder.TimeOfDay = dto.TimeOfDay;
            reminder.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid id)
        {
            var reminder = await _context.Reminders
                .FirstOrDefaultAsync(r => r.ReminderId == id && r.UserId == userId);

            if (reminder == null) return false;

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
