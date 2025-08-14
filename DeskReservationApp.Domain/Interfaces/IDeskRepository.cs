using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Desk entity operations
    /// </summary>
    public interface IDeskRepository : IRepository<Desk>
    {
        Task<IEnumerable<Desk>> GetDesksByFloorIdAsync(int floorId);
        Task<Desk?> GetDeskWithReservationsAsync(int deskId);
        Task<IEnumerable<Desk>> GetAvailableDesksAsync(DateTime startTime, DateTime endTime);
        Task<bool> DeskNameExistsOnFloorAsync(string deskName, int floorId, int? excludeDeskId = null);
    }
}
