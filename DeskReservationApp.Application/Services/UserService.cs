using AutoMapper;
using DeskReservationApp.Application.DTOs.Authentication;
using DeskReservationApp.Application.DTOs.User;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Application.Services
{
    /// <summary>
    /// User service for Windows Authentication users
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper)
        {
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
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException($"User with ID {id} not found");

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new NotFoundException($"User with username {username} not found");

            return _mapper.Map<UserDTO>(user);
        }
    }
}