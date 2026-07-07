using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IFoodEntryService
    {
        Task<IEnumerable<FoodEntryResponseDTO>> GetAllByUserAsync(Guid userId);
        Task<FoodEntryResponseDTO?> GetByIdAsync(Guid id);
        Task<FoodEntryResponseDTO> CreateAsync(FoodEntryCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, FoodEntryUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}