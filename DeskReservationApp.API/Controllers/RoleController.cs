using DeskReservationApp.Application.DTOs.Role;
using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeskReservationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequestDTO request)
        {
            await _roleService.AssignRoleAsync(request);
            return Ok();
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] AssignRoleRequestDTO request)
        {
            await _roleService.RemoveRoleAsync(request);
            return Ok();
        }

        /// <summary>
        /// Get user's roles
        /// </summary>
        [HttpGet("user-roles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var roles = await _roleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet("all-roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDTO request)
        {
            await _roleService.CreateRoleAsync(request);
            return Ok();
        }
    }
}
