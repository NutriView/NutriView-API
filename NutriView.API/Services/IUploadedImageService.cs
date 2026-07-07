using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IUploadedImageService
    {
        Task<IEnumerable<UploadedImageResponseDTO>> GetAllAsync();
        Task<UploadedImageResponseDTO?> GetByIdAsync(Guid id);
        Task<UploadedImageResponseDTO> CreateAsync(UploadedImageCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, UploadedImageUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
