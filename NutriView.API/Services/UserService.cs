using Microsoft.EntityFrameworkCore;
using NutriView.API.Data;
using NutriView.API.Models.DTOs;
using NutriView.API.Models.Entities;
using System.Security.Cryptography;
using System.Text;

namespace NutriView.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    NickName = u.NickName,
                    DailyCalorieGoal = u.DailyCalorieGoal,
                    Weight = u.Weight,
                    Height = u.Height,
                    Age = u.Age,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserResponseDTO?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return null;

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                NickName = user.NickName,
                DailyCalorieGoal = user.DailyCalorieGoal,
                Weight = user.Weight,
                Height = user.Height,
                Age = user.Age,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserResponseDTO> CreateAsync(UserCreateDTO dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (exists)
                throw new Exception("Email already exists");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = dto.Email,
                NickName = dto.NickName,
                PasswordHash = HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                DailyCalorieGoal = dto.DailyCalorieGoal,
                Weight = dto.Weight,
                Height = dto.Height,
                Age = dto.Age
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                NickName = user.NickName,
                DailyCalorieGoal = user.DailyCalorieGoal,
                Weight = user.Weight,
                Height = user.Height,
                Age = user.Age,
                CreatedAt = user.CreatedAt
            };
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

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}