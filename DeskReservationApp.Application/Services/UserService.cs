using AutoMapper;
using DeskReservationApp.Application.DTOs.Authentication;
using DeskReservationApp.Application.DTOs.User;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(
            IIdentityService identityService,
            ITokenService tokenService,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BadRequestException("User ID cannot be null or empty.");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"User with ID '{id}' not found.");
            }
            
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
            if (loginRequest == null)
            {
                throw new BadRequestException("Login request cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(loginRequest.Email))
            {
                throw new BadRequestException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                throw new BadRequestException("Password is required.");
            }

            var userId = await _identityService.GetUserIdByEmailAsync(loginRequest.Email);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var passwordValid = await _identityService.CheckPasswordAsync(loginRequest.Email, loginRequest.Password);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var roles = await _identityService.GetUserRolesAsync(userId);
            var token = _tokenService.GenerateAccessToken(userId, loginRequest.Email, roles);

            return new LoginResponseDTO { JwtToken = token };
        }

        public Task Logout()
        {
            // In a stateless JWT implementation, logout is handled on the client-side.
            return Task.CompletedTask;
        }

        public async Task<RegisterResponseDTO> Register(RegisterRequestDTO registerRequest, bool asAdmin)
        {
            if (registerRequest == null)
            {
                throw new BadRequestException("Registration request cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(registerRequest.Email))
            {
                throw new BadRequestException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(registerRequest.Password))
            {
                throw new BadRequestException("Password is required.");
            }

            // Check if user already exists
            var existingUserId = await _identityService.GetUserIdByEmailAsync(registerRequest.Email);
            if (existingUserId != null)
            {
                throw new BadRequestException($"User with email '{registerRequest.Email}' already exists.");
            }

            var (succeeded, userId) = await _identityService.CreateUserAsync(registerRequest.Email, registerRequest.Password);

            if (!succeeded)
            {
                throw new BadRequestException("Registration failed. Please check your input and try again.");
            }

            var role = asAdmin ? "Admin" : "User";
            await _identityService.AddUserToRoleAsync(userId, role);

            return new RegisterResponseDTO { Email = registerRequest.Email };
        }
    }
}
