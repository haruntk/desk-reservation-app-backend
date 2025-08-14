namespace DeskReservationApp.Application.DTOs.Reservation
{
    /// <summary>
    /// Response DTO for user reservations
    /// </summary>
    public class UserReservationsResponseDTO
    {
        public string UserId { get; set; } = string.Empty;
        public List<ReservationResponseDTO> ActiveReservations { get; set; } = new();
        public List<ReservationResponseDTO> UpcomingReservations { get; set; } = new();
        public List<ReservationResponseDTO> PastReservations { get; set; } = new();
        public int TotalReservations { get; set; }
    }
}
