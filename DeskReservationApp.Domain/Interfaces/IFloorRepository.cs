using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Floor entity operations
    /// </summary>
    public interface IFloorRepository : IRepository<Floor>
    {
        Task<Floor?> GetByFloorNumberAsync(int floorNumber);
        Task<IEnumerable<Floor>> GetFloorsWithDesksAsync();
        Task<bool> FloorNumberExistsAsync(int floorNumber);
    }
}
