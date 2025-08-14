using DeskReservationApp.Application.DTOs.Authentication;
using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeskReservationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Register a new user (default role: User)
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            var response = await _userService.Register(registerRequest, false);
            return Ok(response);
        }

        /// <summary>
        /// Register a new admin user (only existing admins can do this)
        /// </summary>
        [HttpPost("register-admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequestDTO registerRequest)
        {
            var response = await _userService.Register(registerRequest, true);
            return Ok(response);
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var response = await _userService.Login(request);
            return Ok(response);
        }

        /// <summary>
        /// User logout (JWT is stateless; logout is a client-side token discard)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return Ok(new { message = "Logout successful. Please discard your token client-side." });
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
