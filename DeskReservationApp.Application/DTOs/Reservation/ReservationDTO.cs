using DeskReservationApp.Application.DTOs.Desk;

namespace DeskReservationApp.Application.DTOs.Reservation
{
    /// <summary>
    /// Reservation data transfer object
    /// </summary>
    public class ReservationDTO
    {
        public int ReservationId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int DeskId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DeskDTO? Desk { get; set; }
    }
}
