using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IFoodService
    {
        Task<IEnumerable<FoodResponseDTO>> GetAllAsync();
        Task<FoodResponseDTO?> GetByIdAsync(Guid id);
        Task<FoodResponseDTO> CreateAsync(FoodCreateDTO DTO);
        Task<bool> UpdateAsync(Guid id, FoodUpdateDTO DTO);
        Task<bool> DeleteAsync(Guid id);
    }
}