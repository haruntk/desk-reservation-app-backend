using AutoMapper;
using DeskReservationApp.Application.DTOs.User;
using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeskReservationApp.API.Controllers
{
    /// <summary>
    /// Authentication controller for Windows Authentication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IWindowsAuthService _windowsAuthService;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;

        public AuthController(IWindowsAuthService windowsAuthService, ILogger<AuthController> logger, IMapper mapper)
        {
            _windowsAuthService = windowsAuthService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Windows Authentication login - returns current user info
        /// </summary>
        [HttpGet("login")]
        [Authorize]
        public async Task<IActionResult> Login()
        {
            try
            {
                var windowsUsername = User.Identity?.Name;
                if (string.IsNullOrEmpty(windowsUsername))
                {
                    return Unauthorized("Windows username not found");
                }

                _logger.LogInformation("Windows login attempt for user: {Username}", windowsUsername);

                // Get or create user in database
                var user = await _windowsAuthService.GetOrCreateWindowsUserAsync(windowsUsername);

                var userDto = _mapper.Map<UserDTO>(user);

                _logger.LogInformation("Windows login successful for user: {Username} with roles: {Roles}", 
                    windowsUsername, string.Join(", ", userDto.Roles));

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Windows authentication login");
                return StatusCode(500, "Internal server error during authentication");
            }
        }

        /// <summary>
        /// Logout - clears authentication
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // For Windows Authentication, logout is typically handled client-side
            // or by redirecting to a logout URL that clears the authentication cookie
            _logger.LogInformation("Logout requested for user: {Username}", User.Identity?.Name);
            
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var windowsUsername = User.Identity?.Name;
                if (string.IsNullOrEmpty(windowsUsername))
                {
                    return Unauthorized("Windows username not found");
                }

                var user = await _windowsAuthService.GetOrCreateWindowsUserAsync(windowsUsername);

                var userDto = _mapper.Map<UserDTO>(user);

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user info");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
