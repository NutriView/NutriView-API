using Microsoft.EntityFrameworkCore;
using NutriView.API.Data;
using NutriView.API.Exceptions;
using NutriView.API.Models.DTOs;
using NutriView.API.Models.Entities;

namespace NutriView.API.Services
{
    public class UploadedImageService : IUploadedImageService
    {
        private readonly ApplicationDbContext _context;

        public UploadedImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UploadedImageResponseDTO>> GetAllByUserAsync(Guid userId)
        {
            return await _context.UploadedImages
                .Include(i => i.DetectedFood)
                .Where(i => i.UserId == userId)
                .Select(i => new UploadedImageResponseDTO
                {
                    UploadedImageId = i.UploadedImageId,
                    UserId = i.UserId,
                    FilePath = i.FilePath,
                    UploadedAt = i.UploadedAt,
                    IsProcessed = i.IsProcessed,
                    DetectedFoodId = i.DetectedFoodId,
                    DetectedFoodName = i.DetectedFood == null ? null : i.DetectedFood.Name,
                    AIConfidence = i.AIConfidence
                })
                .ToListAsync();
        }

        public async Task<UploadedImageResponseDTO?> GetByIdAsync(Guid userId, Guid id)
        {
            // Scoping by UserId is the ownership check: another user's image is
            // indistinguishable from one that does not exist.
            var image = await _context.UploadedImages
                .Include(i => i.DetectedFood)
                .FirstOrDefaultAsync(i => i.UploadedImageId == id && i.UserId == userId);

            if (image == null) return null;

            return new UploadedImageResponseDTO
            {
                UploadedImageId = image.UploadedImageId,
                UserId = image.UserId,
                FilePath = image.FilePath,
                UploadedAt = image.UploadedAt,
                IsProcessed = image.IsProcessed,
                DetectedFoodId = image.DetectedFoodId,
                DetectedFoodName = image.DetectedFood?.Name,
                AIConfidence = image.AIConfidence
            };
        }

        public async Task<UploadedImageResponseDTO> CreateAsync(Guid userId, UploadedImageCreateDTO dto)
        {
            // A token can outlive the account it was issued for, so the user is still checked.
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
                throw new ValidationException("User not found");

            var image = new UploadedImage
            {
                UploadedImageId = Guid.NewGuid(),
                UserId = userId,
                FilePath = dto.FilePath,
                UploadedAt = DateTime.UtcNow,
                IsProcessed = false
            };

            _context.UploadedImages.Add(image);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(userId, image.UploadedImageId)
                   ?? throw new Exception("Error creating uploaded image");
        }

        public async Task<bool> UpdateAsync(Guid userId, Guid id, UploadedImageUpdateDTO dto)
        {
            var image = await _context.UploadedImages
                .FirstOrDefaultAsync(i => i.UploadedImageId == id && i.UserId == userId);

            if (image == null) return false;

            if (dto.DetectedFoodId.HasValue)
            {
                var foodExists = await _context.Foods.AnyAsync(f => f.FoodId == dto.DetectedFoodId.Value);
                if (!foodExists)
                    throw new ValidationException("Detected food not found");
            }

            image.IsProcessed = dto.IsProcessed;
            image.DetectedFoodId = dto.DetectedFoodId;
            image.AIConfidence = dto.AIConfidence;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid id)
        {
            var image = await _context.UploadedImages
                .FirstOrDefaultAsync(i => i.UploadedImageId == id && i.UserId == userId);

            if (image == null) return false;

            _context.UploadedImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
