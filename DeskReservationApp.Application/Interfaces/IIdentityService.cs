using DeskReservationApp.Application.DTOs.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskReservationApp.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<(bool Succeeded, string UserId)> CreateUserAsync(string email, string password);
        Task AddUserToRoleAsync(string userId, string roleName);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<string?> GetUserIdByEmailAsync(string email);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<bool> RoleExistsAsync(string roleName);
        Task CreateRoleAsync(string roleName);
        Task<IList<string>> GetAllRolesAsync();
        Task AssignUserToRoleAsync(string userId, string roleName);
        Task RemoveUserFromRoleAsync(string userId, string roleName);
        Task<string?> GetUserNameByIdAsync(string userId);
    }
}
