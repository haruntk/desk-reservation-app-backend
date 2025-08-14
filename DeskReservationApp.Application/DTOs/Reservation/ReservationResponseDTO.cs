namespace DeskReservationApp.Application.DTOs.Reservation
{
    /// <summary>
    /// Response DTO for reservation operations
    /// </summary>
    public class ReservationResponseDTO
    {
        public int ReservationId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int DeskId { get; set; }
        public string DeskName { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public bool IsActive => Status == "Active";
        public bool IsPast => EndTime < DateTime.UtcNow;
        public bool IsUpcoming => StartTime > DateTime.UtcNow && Status == "Active";
    }
}
