using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderResponseDTO>> GetAllByUserAsync(Guid userId);
        Task<ReminderResponseDTO?> GetByIdAsync(Guid userId, Guid id);
        Task<ReminderResponseDTO> CreateAsync(Guid userId, ReminderCreateDTO dto);
        Task<bool> UpdateAsync(Guid userId, Guid id, ReminderUpdateDTO dto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
