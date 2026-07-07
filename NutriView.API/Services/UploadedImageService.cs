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

        public async Task<IEnumerable<UploadedImageResponseDTO>> GetAllAsync()
        {
            return await _context.UploadedImages
                .Include(i => i.DetectedFood)
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

        public async Task<UploadedImageResponseDTO?> GetByIdAsync(Guid id)
        {
            var image = await _context.UploadedImages
                .Include(i => i.DetectedFood)
                .FirstOrDefaultAsync(i => i.UploadedImageId == id);

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

        public async Task<UploadedImageResponseDTO> CreateAsync(UploadedImageCreateDTO dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                throw new ValidationException("User not found");

            var image = new UploadedImage
            {
                UploadedImageId = Guid.NewGuid(),
                UserId = dto.UserId,
                FilePath = dto.FilePath,
                UploadedAt = DateTime.UtcNow,
                IsProcessed = false
            };

            _context.UploadedImages.Add(image);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(image.UploadedImageId)
                   ?? throw new Exception("Error creating uploaded image");
        }

        public async Task<bool> UpdateAsync(Guid id, UploadedImageUpdateDTO dto)
        {
            var image = await _context.UploadedImages.FindAsync(id);
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            var image = await _context.UploadedImages.FindAsync(id);
            if (image == null) return false;

            _context.UploadedImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
