using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DeskReservationApp.Infrastructure.Services
{
    /// <summary>
    /// Claims transformation service to add database roles to Windows authenticated users
    /// </summary>
    public class WindowsClaimsTransformation : IClaimsTransformation
    {
        private readonly IWindowsAuthService _windowsAuthService;
        private readonly ILogger<WindowsClaimsTransformation> _logger;

        public WindowsClaimsTransformation(
            IWindowsAuthService windowsAuthService,
            ILogger<WindowsClaimsTransformation> logger)
        {
            _windowsAuthService = windowsAuthService;
            _logger = logger;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Only transform if user is authenticated via Windows Authentication
            if (!principal.Identity?.IsAuthenticated == true || 
                principal.Identity.AuthenticationType != "Negotiate")
            {
                return principal;
            }

            var identity = (ClaimsIdentity)principal.Identity;
            var windowsUsername = identity.Name;

            if (string.IsNullOrEmpty(windowsUsername))
            {
                _logger.LogWarning("Windows username is null or empty");
                return principal;
            }

            try
            {
                // Get user roles from database
                var roles = await _windowsAuthService.GetUserRolesAsync(windowsUsername);

                // Create new identity with existing claims plus role claims
                var newIdentity = new ClaimsIdentity(identity.Claims, identity.AuthenticationType);

                // Add role claims
                foreach (var role in roles)
                {
                    if (!newIdentity.HasClaim(ClaimTypes.Role, role))
                    {
                        newIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }

                // Ensure user exists in database
                var user = await _windowsAuthService.GetOrCreateWindowsUserAsync(windowsUsername);
                
                // Add user ID claim if not present
                if (!newIdentity.HasClaim(ClaimTypes.NameIdentifier, user.Id))
                {
                    newIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                }

                // Add email claim if not present
                if (!newIdentity.HasClaim(ClaimTypes.Email, user.Email))
                {
                    newIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                }

                _logger.LogInformation("Successfully transformed claims for user {Username} with roles: {Roles}", 
                    windowsUsername, string.Join(", ", roles));

                return new ClaimsPrincipal(newIdentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transforming claims for user {Username}", windowsUsername);
                return principal;
            }
        }
    }
}
