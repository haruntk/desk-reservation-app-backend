using DeskReservationApp.Domain.Entities;
using System.Security.Claims;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Service interface for Windows Authentication operations
    /// </summary>
    public interface IWindowsAuthService
    {
        /// <summary>
        /// Get user roles from database based on Windows username
        /// </summary>
        /// <param name="windowsUsername">Windows username (DOMAIN\username format)</param>
        /// <returns>List of role names</returns>
        Task<IList<string>> GetUserRolesAsync(string windowsUsername);

        /// <summary>
        /// Get or create user information based on Windows authentication
        /// </summary>
        /// <param name="windowsUsername">Windows username (DOMAIN\username format)</param>
        /// <returns>User information</returns>
        Task<User> GetOrCreateWindowsUserAsync(string windowsUsername);

        /// <summary>
        /// Assign role to Windows user
        /// </summary>
        /// <param name="windowsUsername">Windows username (DOMAIN\username format)</param>
        /// <param name="roleName">Role name to assign</param>
        Task AssignRoleToWindowsUserAsync(string windowsUsername, string roleName);

        /// <summary>
        /// Remove role from Windows user
        /// </summary>
        /// <param name="windowsUsername">Windows username (DOMAIN\username format)</param>
        /// <param name="roleName">Role name to remove</param>
        Task RemoveRoleFromWindowsUserAsync(string windowsUsername, string roleName);
    }
}
