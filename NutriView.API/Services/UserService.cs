using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NutriView.API.Data;
using NutriView.API.Exceptions;
using NutriView.API.Models.DTOs;
using NutriView.API.Models.Entities;

namespace NutriView.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly INutritionService _nutritionService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public UserService(
            ApplicationDbContext context,
            INutritionService nutritionService,
            IPasswordHasher<User> passwordHasher,
            ITokenService tokenService)
        {
            _context = context;
            _nutritionService = nutritionService;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<UserResponseDTO?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.NutritionDailyGoal)
                .FirstOrDefaultAsync(u => u.UserId == id);

            return user == null ? null : MapUser(user);
        }

        public async Task<AuthResponseDTO> RegisterAsync(UserCreateDTO dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (exists)
                throw new ValidationException("Email already exists");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = dto.Email,
                NickName = dto.NickName,
                CreatedAt = DateTime.UtcNow,
                DailyCalorieGoal = dto.DailyCalorieGoal,
                Weight = dto.Weight,
                Height = dto.Height,
                Age = dto.Age,
                Gender = dto.Gender,
                Image = dto.Image
            };

            // PBKDF2-HMAC-SHA512 with a per-user random salt embedded in the output,
            // so two users with the same password never share a hash.
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.NutritionDailyGoal)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            // The hasher's work factor was raised since this hash was written: upgrade it
            // now that the plaintext is available.
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
                await _context.SaveChangesAsync();
            }

            return BuildAuthResponse(user);
        }

        public async Task<bool> UpdateAsync(Guid id, UserUpdateDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.NickName = dto.NickName;
            user.DailyCalorieGoal = dto.DailyCalorieGoal;
            user.Weight = dto.Weight;
            user.Height = dto.Height;
            user.Age = dto.Age;
            user.Gender = dto.Gender;
            user.Image = dto.Image;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.NutritionDailyGoal)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return false;

            // The user holds the FK to its goal, so removing the user alone would
            // orphan the standalone goal row. Remove both.
            if (user.NutritionDailyGoal != null)
                _context.NutritionValues.Remove(user.NutritionDailyGoal);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<NutritionValueDTO?> GetNutritionGoalAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.NutritionDailyGoal)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user?.NutritionDailyGoal == null) return null;

            return MapNutrition(user.NutritionDailyGoal);
        }

        public async Task<bool> SetNutritionGoalAsync(Guid id, NutritionValueDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.NutritionDailyGoal)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return false;

            var goal = user.NutritionDailyGoal;
            if (goal == null)
            {
                goal = new NutritionValue { NutritionValueId = Guid.NewGuid() };
                user.NutritionDailyGoal = goal;
                // Force Added state: attaching a new entity with a non-empty
                // generated key to a tracked user's navigation would otherwise be
                // treated as an UPDATE and throw a concurrency exception.
                _context.NutritionValues.Add(goal);
            }

            // FoodId stays null: this is a standalone goal, not a food's macros.
            goal.Calories = _nutritionService.CalculateCalories(
                dto.Protein, dto.Carbs, dto.Fat, dto.Fiber, dto.Alcohol);
            goal.Protein = dto.Protein;
            goal.Carbs = dto.Carbs;
            goal.Fat = dto.Fat;
            goal.Sugar = dto.Sugar;
            goal.Fiber = dto.Fiber;
            goal.Sodium = dto.Sodium;
            goal.Alcohol = dto.Alcohol;
            goal.MeasurementBase = dto.MeasurementBase;

            await _context.SaveChangesAsync();
            return true;
        }

        private AuthResponseDTO BuildAuthResponse(User user)
        {
            var token = _tokenService.CreateToken(user);

            return new AuthResponseDTO
            {
                Token = token.Token,
                ExpiresAt = token.ExpiresAt,
                User = MapUser(user)
            };
        }

        private static UserResponseDTO MapUser(User user) => new()
        {
            UserId = user.UserId,
            Email = user.Email,
            NickName = user.NickName,
            DailyCalorieGoal = user.DailyCalorieGoal,
            Weight = user.Weight,
            Height = user.Height,
            Age = user.Age,
            Gender = user.Gender,
            Image = user.Image,
            CreatedAt = user.CreatedAt,
            NutritionDailyGoal = user.NutritionDailyGoal == null
                ? null
                : MapNutrition(user.NutritionDailyGoal)
        };

        private static NutritionValueDTO MapNutrition(NutritionValue n) => new()
        {
            Calories = n.Calories,
            Protein = n.Protein,
            Carbs = n.Carbs,
            Fat = n.Fat,
            Sugar = n.Sugar,
            Fiber = n.Fiber,
            Sodium = n.Sodium,
            Alcohol = n.Alcohol,
            MeasurementBase = n.MeasurementBase
        };
    }
}
