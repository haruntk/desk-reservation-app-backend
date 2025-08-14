using DeskReservationApp.Application.DTOs.Floor;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Floor service interface for business logic operations
    /// </summary>
    public interface IFloorService
    {
        Task<IEnumerable<FloorResponseDTO>> GetAllFloorsAsync();
        Task<FloorDTO> GetFloorByIdAsync(int floorId);
        Task<FloorDTO> GetFloorWithDesksAsync(int floorId);
        Task<IEnumerable<FloorDTO>> GetFloorsWithDesksAsync();
        Task<int> CreateFloorAsync(CreateFloorRequestDTO createFloorRequest);
        Task UpdateFloorAsync(int floorId, UpdateFloorRequestDTO updateFloorRequest);
        Task DeleteFloorAsync(int floorId);
    }
}
