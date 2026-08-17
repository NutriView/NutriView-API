using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO?> GetByIdAsync(Guid id);
        Task<AuthResponseDTO> RegisterAsync(UserCreateDTO dto);
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
        Task<bool> UpdateAsync(Guid id, UserUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<NutritionValueDTO?> GetNutritionGoalAsync(Guid id);
        Task<bool> SetNutritionGoalAsync(Guid id, NutritionValueDTO dto);
    }
}
