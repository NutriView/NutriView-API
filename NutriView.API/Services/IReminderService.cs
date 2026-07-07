using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderResponseDTO>> GetAllByUserAsync(Guid userId);
        Task<ReminderResponseDTO?> GetByIdAsync(Guid id);
        Task<ReminderResponseDTO> CreateAsync(ReminderCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ReminderUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}