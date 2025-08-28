using AutoMapper;
using DeskReservationApp.Application.DTOs.User;
using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeskReservationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWindowsAuthService _windowsAuthService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IWindowsAuthService windowsAuthService, IMapper mapper)
        {
            _userService = userService;
            _windowsAuthService = windowsAuthService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet("get-all")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _userService.GetAllAsync();
            return Ok(response);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
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

        /// <summary>
        /// Assign role to Windows user (Admin only)
        /// </summary>
        [HttpPost("{username}/assign-role/{roleName}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AssignRole(string username, string roleName)
        {
            try
            {
                await _windowsAuthService.AssignRoleToWindowsUserAsync(username, roleName);
                return Ok(new { message = $"Role '{roleName}' assigned to user '{username}' successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove role from Windows user (Admin only)
        /// </summary>
        [HttpDelete("{username}/remove-role/{roleName}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RemoveRole(string username, string roleName)
        {
            try
            {
                await _windowsAuthService.RemoveRoleFromWindowsUserAsync(username, roleName);
                return Ok(new { message = $"Role '{roleName}' removed from user '{username}' successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _userService.GetByIdAsync(id);
            return Ok(response);
        }
    }
}
