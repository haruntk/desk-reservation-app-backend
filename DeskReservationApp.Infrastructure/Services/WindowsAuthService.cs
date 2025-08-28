using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Domain.Configuration;
using DeskReservationApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace DeskReservationApp.Infrastructure.Services
{
    /// <summary>
    /// Service for Windows Authentication operations
    /// </summary>
    public class WindowsAuthService : IWindowsAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly WindowsAuthOptions _options;
        private readonly ILogger<WindowsAuthService> _logger;

        public WindowsAuthService(
            IUserRepository userRepository, 
            IAuthUnitOfWork authUnitOfWork, 
            IRoleRepository roleRepository,
            IOptions<WindowsAuthOptions> options,
            ILogger<WindowsAuthService> logger)
        {
            _userRepository = userRepository;
            _authUnitOfWork = authUnitOfWork;
            _roleRepository = roleRepository;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<IList<string>> GetUserRolesAsync(string windowsUsername)
        {
            if (string.IsNullOrEmpty(windowsUsername))
                return new List<string>();

            var normalizedUsername = NormalizeWindowsUsername(windowsUsername);
            var user = await _userRepository.GetByUsernameAsync(normalizedUsername);
            
            if (user == null)
                return new List<string>();

            return user.Roles.Select(ur => ur.Role.Name).ToList();
        }

        public async Task<User> GetOrCreateWindowsUserAsync(string windowsUsername)
        {
            if (string.IsNullOrEmpty(windowsUsername))
                throw new ArgumentException("Windows username cannot be null or empty", nameof(windowsUsername));

            var normalizedUsername = NormalizeWindowsUsername(windowsUsername);
            var user = await _userRepository.GetByUsernameAsync(normalizedUsername);

            if (user == null)
            {
                if (_options.EnableDetailedLogging)
                {
                    _logger.LogInformation("Creating new Windows user: {Username}", normalizedUsername);
                }

                user = new User(normalizedUsername, ExtractEmailFromUsername(normalizedUsername));
                await _userRepository.AddAsync(user);
                await _authUnitOfWork.SaveChangesAsync();

                // Assign default role to new user
                if (!string.IsNullOrEmpty(_options.DefaultUserRole))
                {
                    await AssignDefaultRoleToNewUserAsync(user, _options.DefaultUserRole);
                    
                    if (_options.EnableDetailedLogging)
                    {
                        _logger.LogInformation("Assigned default role '{Role}' to new user: {Username}", 
                            _options.DefaultUserRole, normalizedUsername);
                    }
                }
            }

            return user;
        }

        public async Task AssignRoleToWindowsUserAsync(string windowsUsername, string roleName)
        {
            if (string.IsNullOrEmpty(windowsUsername) || string.IsNullOrEmpty(roleName))
                return;

            var normalizedUsername = NormalizeWindowsUsername(windowsUsername);
            var user = await _userRepository.GetByUsernameAsync(normalizedUsername);

            if (user == null)
                return;

            var role = await _roleRepository.GetByNameAsync(roleName);

            if (role == null)
            {
                role = new Role(roleName);
                await _roleRepository.AddAsync(role);
                await _authUnitOfWork.SaveChangesAsync();
            }
            
            user.AssignRole(role);
            await _userRepository.UpdateAsync(user);
            await _authUnitOfWork.SaveChangesAsync();
        }

        public async Task RemoveRoleFromWindowsUserAsync(string windowsUsername, string roleName)
        {
            if (string.IsNullOrEmpty(windowsUsername) || string.IsNullOrEmpty(roleName))
                return;

            var normalizedUsername = NormalizeWindowsUsername(windowsUsername);
            var user = await _userRepository.GetByUsernameAsync(normalizedUsername);

            if (user == null) return;

            var role = await _roleRepository.GetByNameAsync(roleName);

            if (role == null) return;

            user.RemoveRole(role);
            await _userRepository.UpdateAsync(user);
            await _authUnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Normalize Windows username to consistent format
        /// </summary>
        /// <param name="windowsUsername">Windows username (can be DOMAIN\username or username@domain format)</param>
        /// <returns>Normalized username</returns>
        private string NormalizeWindowsUsername(string windowsUsername)
        {
            if (string.IsNullOrEmpty(windowsUsername))
                return string.Empty;

            // Remove domain part and return just username
            if (windowsUsername.Contains("\\"))
            {
                return windowsUsername.Split('\\')[1].ToLowerInvariant();
            }
            
            if (windowsUsername.Contains("@"))
            {
                return windowsUsername.Split('@')[0].ToLowerInvariant();
            }

            return windowsUsername.ToLowerInvariant();
        }

        /// <summary>
        /// Extract or generate email from Windows username
        /// </summary>
        /// <param name="username">Normalized username</param>
        /// <returns>Email address</returns>
        private string ExtractEmailFromUsername(string username)
        {
            // For now, we'll create a default email format
            // In a real scenario, you might query Active Directory for the actual email
            return $"{username}@{_options.DefaultEmailDomain}";
        }

        /// <summary>
        /// Assign default role to a newly created user
        /// </summary>
        /// <param name="user">The newly created user</param>
        /// <param name="defaultRoleName">Default role name to assign</param>
        private async Task AssignDefaultRoleToNewUserAsync(User user, string defaultRoleName)
        {
            try
            {
                // Get or create the default role
                var role = await _roleRepository.GetByNameAsync(defaultRoleName);
                if (role == null)
                {
                    role = new Role(defaultRoleName);
                    await _roleRepository.AddAsync(role);
                    await _authUnitOfWork.SaveChangesAsync();
                    
                    if (_options.EnableDetailedLogging)
                    {
                        _logger.LogInformation("Created new role: {RoleName}", defaultRoleName);
                    }
                }

                // Assign the role to the user using domain method
                user.AssignRole(role);
                await _userRepository.UpdateAsync(user);
                await _authUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign default role '{RoleName}' to user '{Username}'", 
                    defaultRoleName, user.UserName);
                // Don't throw - user creation should still succeed even if role assignment fails
            }
        }
    }
}
