using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Reservation entity operations
    /// </summary>
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId);
        Task<IEnumerable<Reservation>> GetReservationsByDeskIdAsync(int deskId);
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
        Task<IEnumerable<Reservation>> GetPastReservationsAsync(string userId);
        Task<IEnumerable<Reservation>> GetUpcomingReservationsAsync(string userId);
        Task<bool> HasOverlappingReservationAsync(int deskId, DateTime startTime, DateTime endTime, int? excludeReservationId = null);
        Task<IEnumerable<Reservation>> GetReservationsWithDetailsAsync();
    }
}
