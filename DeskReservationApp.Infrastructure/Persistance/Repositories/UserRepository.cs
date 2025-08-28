using Microsoft.EntityFrameworkCore;
using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// User repository implementation for Windows Authentication users
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DeskReservationAuthDbContext _dbContext;

        public UserRepository(DeskReservationAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
        }

        public async Task DeleteAsync(string id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
            }
        }
    }
}
