using DeskReservationApp.Application.DTOs.Reservation;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Reservation service interface for business logic operations
    /// </summary>
    public interface IReservationService
    {
        Task<IEnumerable<ReservationResponseDTO>> GetAllReservationsAsync();
        Task<ReservationDTO> GetReservationByIdAsync(int reservationId);
        Task<UserReservationsResponseDTO> GetUserReservationsAsync(string userId);
        Task<IEnumerable<ReservationResponseDTO>> GetReservationsByDeskIdAsync(int deskId);
        Task<IEnumerable<ReservationResponseDTO>> GetActiveReservationsAsync();
        Task<IEnumerable<ReservationResponseDTO>> GetPastReservationsAsync(string userId);
        Task<IEnumerable<ReservationResponseDTO>> GetUpcomingReservationsAsync(string userId);
        Task<int> CreateReservationAsync(string userId, CreateReservationRequestDTO createReservationRequest);
        Task UpdateReservationAsync(int reservationId, string userId, UpdateReservationRequestDTO updateReservationRequest);
        Task UpdateReservationStatusAsync(int reservationId, string userId, UpdateReservationStatusRequestDTO updateStatusRequest);
        Task CancelReservationAsync(int reservationId, string userId);
        Task DeleteReservationAsync(int reservationId);
    }
}
