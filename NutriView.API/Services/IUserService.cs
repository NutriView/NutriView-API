using NutriView.API.Models.DTOs;

namespace NutriView.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO?> GetByIdAsync(Guid id);
        Task<UserResponseDTO> CreateAsync(UserCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, UserUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}