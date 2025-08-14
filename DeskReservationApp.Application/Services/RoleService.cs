using DeskReservationApp.Application.DTOs.Role;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;


        public RoleService(IIdentityService identityService, IUserRepository userRepository)
        {
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task AssignRoleAsync(AssignRoleRequestDTO request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (!await _identityService.RoleExistsAsync(request.RoleName))
            {
                throw new BadRequestException($"Role '{request.RoleName}' does not exist.");
            }

            await _identityService.AssignUserToRoleAsync(user.Id, request.RoleName);
        }

        public async Task CreateRoleAsync(CreateRoleRequestDTO request)
        {
            if (await _identityService.RoleExistsAsync(request.RoleName))
            {
                throw new BadRequestException($"Role '{request.RoleName}' already exists.");
            }

            await _identityService.CreateRoleAsync(request.RoleName);
        }

        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await _identityService.GetAllRolesAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            return await _identityService.GetUserRolesAsync(user.Id);
        }

        public async Task RemoveRoleAsync(AssignRoleRequestDTO request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (!await _identityService.RoleExistsAsync(request.RoleName))
            {
                throw new BadRequestException($"Role '{request.RoleName}' does not exist.");
            }

            await _identityService.RemoveUserFromRoleAsync(user.Id, request.RoleName);
        }
    }
}
