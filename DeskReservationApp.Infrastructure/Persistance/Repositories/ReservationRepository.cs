using Microsoft.EntityFrameworkCore;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Reservation repository implementation with specific reservation operations
    /// </summary>
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(DeskReservationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByDeskIdAsync(int deskId)
        {
            return await _dbSet
                .Where(r => r.DeskId == deskId)
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
        {
            return await _dbSet
                .Where(r => r.Status == "Active")
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetPastReservationsAsync(string userId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(r => r.UserId == userId &&
                           (r.EndTime < now || r.Status == "Completed" || r.Status == "Cancelled"))
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .OrderByDescending(r => r.EndTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetUpcomingReservationsAsync(string userId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(r => r.UserId == userId &&
                           r.StartTime > now &&
                           r.Status == "Active")
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingReservationAsync(int deskId, DateTime startTime, DateTime endTime, int? excludeReservationId = null)
        {
            var query = _dbSet.Where(r => r.DeskId == deskId &&
                                     r.Status == "Active" &&
                                     (r.StartTime <= startTime && r.EndTime > startTime ||
                                      r.StartTime < endTime && r.EndTime >= endTime ||
                                      r.StartTime >= startTime && r.EndTime <= endTime));

            if (excludeReservationId.HasValue)
            {
                query = query.Where(r => r.ReservationId != excludeReservationId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(r => r.Desk)
                .ThenInclude(d => d.Floor)
                .FirstOrDefaultAsync(r => r.ReservationId == id);
        }
    }
}
