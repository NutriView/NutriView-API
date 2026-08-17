using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IUploadedImageService
    {
        Task<IEnumerable<UploadedImageResponseDTO>> GetAllByUserAsync(Guid userId);
        Task<UploadedImageResponseDTO?> GetByIdAsync(Guid userId, Guid id);
        Task<UploadedImageResponseDTO> CreateAsync(Guid userId, UploadedImageCreateDTO dto);
        Task<bool> UpdateAsync(Guid userId, Guid id, UploadedImageUpdateDTO dto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
