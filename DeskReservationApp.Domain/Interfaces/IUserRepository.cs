using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Domain.Interfaces
{
    /// <summary>
    /// User repository interface for user operations
    /// </summary>
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
    }
}
