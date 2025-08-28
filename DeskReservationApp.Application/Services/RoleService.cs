using DeskReservationApp.Application.DTOs.Role;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Application.Services
{
    /// <summary>
    /// Role service for Windows Authentication
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWindowsAuthService _windowsAuthService;

        public RoleService(IUserRepository userRepository, IWindowsAuthService windowsAuthService)
        {
            _userRepository = userRepository;
            _windowsAuthService = windowsAuthService;
        }

        public async Task AssignRoleAsync(AssignRoleRequestDTO request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // For Windows Authentication, we use predefined roles
            var validRoles = new[] { "User", "TeamLead", "Admin" };
            if (!validRoles.Contains(request.RoleName))
            {
                throw new BadRequestException($"Role '{request.RoleName}' is not a valid role. Valid roles are: {string.Join(", ", validRoles)}");
            }

            await _windowsAuthService.AssignRoleToWindowsUserAsync(user.UserName, request.RoleName);
        }

        public async Task CreateRoleAsync(CreateRoleRequestDTO request)
        {
            // For Windows Authentication, roles are predefined
            await Task.CompletedTask;
            throw new NotImplementedException("Creating roles is not supported with Windows Authentication. Roles are predefined: User, TeamLead, Admin.");
        }

        public async Task RemoveRoleAsync(AssignRoleRequestDTO request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            await _windowsAuthService.RemoveRoleFromWindowsUserAsync(user.UserName, request.RoleName);
        }

        public async Task<ICollection<UserRole>> GetUserRolesAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) 
            {
                throw new NotFoundException("User not found.");
            }

            return user.Roles;
        }

        Task<IList<UserRole>> IRoleService.GetAllRolesAsync()
        {
            throw new NotImplementedException();
        }
    }
}