using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeskReservationApp.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Role repository implementation for Windows Authentication
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly DeskReservationAuthDbContext _dbContext;
        
        public RoleRepository(DeskReservationAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Role role)
        {
            await _dbContext.AddAsync(role);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(string id)
        {
            return await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
