using DeskReservationApp.Application.DTOs.Role;
using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Role service interface for role management operations
    /// </summary>
    public interface IRoleService
    {
        Task AssignRoleAsync(AssignRoleRequestDTO request);
        Task RemoveRoleAsync(AssignRoleRequestDTO request);
        Task<ICollection<UserRole>> GetUserRolesAsync(string userId);
        Task<IList<UserRole>> GetAllRolesAsync();
        Task CreateRoleAsync(CreateRoleRequestDTO request);
    }
}
