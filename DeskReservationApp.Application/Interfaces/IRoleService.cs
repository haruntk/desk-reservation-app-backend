using DeskReservationApp.Application.DTOs.Role;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Role service interface for role management operations
    /// </summary>
    public interface IRoleService
    {
        Task AssignRoleAsync(AssignRoleRequestDTO request);
        Task RemoveRoleAsync(AssignRoleRequestDTO request);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IList<string>> GetAllRolesAsync();
        Task CreateRoleAsync(CreateRoleRequestDTO request);
    }
}
