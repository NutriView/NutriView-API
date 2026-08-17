using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IFoodEntryService
    {
        Task<IEnumerable<FoodEntryResponseDTO>> GetAllByUserAsync(Guid userId);
        Task<FoodEntryResponseDTO?> GetByIdAsync(Guid userId, Guid id);
        Task<FoodEntryResponseDTO> CreateAsync(Guid userId, FoodEntryCreateDTO dto);
        Task<bool> UpdateAsync(Guid userId, Guid id, FoodEntryUpdateDTO dto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
