using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Domain.Interfaces
{
    /// <summary>
    /// User repository interface for Identity user operations
    /// </summary>
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
    }
}
