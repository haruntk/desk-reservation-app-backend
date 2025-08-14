using DeskReservationApp.Application.DTOs.Desk;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Desk service interface for business logic operations
    /// </summary>
    public interface IDeskService
    {
        Task<IEnumerable<DeskResponseDTO>> GetAllDesksAsync();
        Task<DeskDTO> GetDeskByIdAsync(int deskId);
        Task<IEnumerable<DeskResponseDTO>> GetDesksByFloorIdAsync(int floorId);
        Task<IEnumerable<DeskResponseDTO>> GetAvailableDesksAsync(DeskAvailabilityRequestDTO availabilityRequest);
        Task<int> CreateDeskAsync(CreateDeskRequestDTO createDeskRequest);
        Task UpdateDeskAsync(int deskId, UpdateDeskRequestDTO updateDeskRequest);
        Task DeleteDeskAsync(int deskId);
    }
}
