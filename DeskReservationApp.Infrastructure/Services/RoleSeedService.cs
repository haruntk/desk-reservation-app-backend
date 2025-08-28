using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Configuration;
using DeskReservationApp.Infrastructure.Persistance;

namespace DeskReservationApp.Infrastructure.Services
{
    /// <summary>
    /// Service to seed initial data for Windows Authentication
    /// </summary>
    public class RoleSeedService
    {
        private readonly ILogger<RoleSeedService> _logger;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly WindowsAuthOptions _options;

        public RoleSeedService(
            ILogger<RoleSeedService> logger, 
            IRoleRepository roleRepository, 
            IAuthUnitOfWork authUnitOfWork,
            IOptions<WindowsAuthOptions> options)
        {
            _logger = logger;
            _roleRepository = roleRepository;
            _authUnitOfWork = authUnitOfWork;
            _options = options.Value;
        }

        public async Task SeedRolesAsync()
        {
            _logger.LogInformation("Starting Windows Authentication role seeding...");
            
            foreach (var roleName in _options.PredefinedRoles)
            {
                var existingRole = await _roleRepository.GetByNameAsync(roleName);
                if (existingRole == null)
                {
                    var role = new Role(roleName);
                    await _roleRepository.AddAsync(role);
                    _logger.LogInformation("Created role: {RoleName}", roleName);
                }
            }

            await _authUnitOfWork.SaveChangesAsync();
            _logger.LogInformation("Windows Authentication role seeding completed with {RoleCount} roles", _options.PredefinedRoles.Length);
        }
    }
}