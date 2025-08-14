using Microsoft.EntityFrameworkCore;
using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// User repository implementation for Identity user operations
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
            var users = await _dbContext.Users
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Roles = (from ur in _dbContext.UserRoles
                             join r in _dbContext.Roles on ur.RoleId equals r.Id
                             where ur.UserId == u.Id
                             select r.Name)
                            .ToList()
                })
                .ToListAsync();

            return users;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Roles = (from ur in _dbContext.UserRoles
                             join r in _dbContext.Roles on ur.RoleId equals r.Id
                             where ur.UserId == u.Id
                             select r.Name)
                            .ToList()
                })
                .FirstOrDefaultAsync();

            return user;
        }
    }
}
