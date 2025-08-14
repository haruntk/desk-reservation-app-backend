using Microsoft.EntityFrameworkCore;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Desk repository implementation with specific desk operations
    /// </summary>
    public class DeskRepository : Repository<Desk>, IDeskRepository
    {
        public DeskRepository(DeskReservationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Desk>> GetDesksByFloorIdAsync(int floorId)
        {
            return await _dbSet
                .Where(d => d.FloorId == floorId)
                .Include(d => d.Floor)
                .OrderBy(d => d.DeskName)
                .ToListAsync();
        }

        public async Task<Desk?> GetDeskWithReservationsAsync(int deskId)
        {
            return await _dbSet
                .Include(d => d.Floor)
                .Include(d => d.Reservations.Where(r => r.Status == "Active"))
                .FirstOrDefaultAsync(d => d.DeskId == deskId);
        }

        public async Task<IEnumerable<Desk>> GetAvailableDesksAsync(DateTime startTime, DateTime endTime)
        {
            var busyDeskIds = await _context.Reservations
                .Where(r => r.Status == "Active" &&
                           (r.StartTime <= startTime && r.EndTime > startTime ||
                            r.StartTime < endTime && r.EndTime >= endTime ||
                            r.StartTime >= startTime && r.EndTime <= endTime))
                .Select(r => r.DeskId)
                .Distinct()
                .ToListAsync();

            return await _dbSet
                .Where(d => !busyDeskIds.Contains(d.DeskId))
                .Include(d => d.Floor)
                .OrderBy(d => d.Floor.FloorNumber)
                .ThenBy(d => d.DeskName)
                .ToListAsync();
        }

        public async Task<bool> DeskNameExistsOnFloorAsync(string deskName, int floorId, int? excludeDeskId = null)
        {
            var query = _dbSet.Where(d => d.DeskName == deskName && d.FloorId == floorId);

            if (excludeDeskId.HasValue)
            {
                query = query.Where(d => d.DeskId != excludeDeskId.Value);
            }

            return await query.AnyAsync();
        }

        public override async Task<IEnumerable<Desk>> GetAllAsync()
        {
            return await _dbSet
                .Include(d => d.Floor)
                .OrderBy(d => d.Floor.FloorNumber)
                .ThenBy(d => d.DeskName)
                .ToListAsync();
        }

        public override async Task<Desk?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Floor)
                .FirstOrDefaultAsync(d => d.DeskId == id);
        }
    }
}
