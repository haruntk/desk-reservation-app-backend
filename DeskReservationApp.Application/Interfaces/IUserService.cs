using DeskReservationApp.Application.DTOs.Authentication;
using DeskReservationApp.Application.DTOs.User;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// User service interface for authentication and user management
    /// </summary>
    public interface IUserService
    {
        Task<RegisterResponseDTO> Register(RegisterRequestDTO registerRequest, bool asAdmin);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
        Task Logout();
        Task<List<UserDTO>> GetAllAsync();
        Task<UserDTO> GetByIdAsync(string id);
    }
}
