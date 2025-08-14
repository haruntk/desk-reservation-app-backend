using Microsoft.EntityFrameworkCore;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Floor repository implementation with specific floor operations
    /// </summary>
    public class FloorRepository : Repository<Floor>, IFloorRepository
    {
        public FloorRepository(DeskReservationDbContext context) : base(context)
        {
        }

        public async Task<Floor?> GetByFloorNumberAsync(int floorNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.FloorNumber == floorNumber);
        }

        public async Task<IEnumerable<Floor>> GetFloorsWithDesksAsync()
        {
            return await _dbSet
                .Include(f => f.Desks)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public async Task<bool> FloorNumberExistsAsync(int floorNumber)
        {
            return await _dbSet.AnyAsync(f => f.FloorNumber == floorNumber);
        }

        public override async Task<IEnumerable<Floor>> GetAllAsync()
        {
            return await _dbSet
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public override async Task<Floor?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(f => f.Desks)
                .FirstOrDefaultAsync(f => f.FloorId == id);
        }
    }
}
