using DeskReservationApp.Application.DTOs.Authentication;
using DeskReservationApp.Application.DTOs.User;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// User service interface for Windows Authentication user management
    /// </summary>
    public interface IUserService
    {
        
        // User management methods
        Task<List<UserDTO>> GetAllAsync();
        Task<UserDTO> GetByIdAsync(string id);
        Task<UserDTO> GetByUsernameAsync(string username);
    }
}
