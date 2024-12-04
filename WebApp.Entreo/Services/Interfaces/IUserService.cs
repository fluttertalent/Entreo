using WebApp.Entreo.Shared.Models;
using WebApp.Entreo.Shared.Models.DTOs;

namespace WebApp.Entreo.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> DeleteUserAsync(string userId);
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> UpdateUserAsync(string userId, UserUpdateDto updateDto);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ReactivateUserAsync(string userId);
        Task<bool> UpdateUserRolesAsync(string userId, IEnumerable<string> roles);
    }
}