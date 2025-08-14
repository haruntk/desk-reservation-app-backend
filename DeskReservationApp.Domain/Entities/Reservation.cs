namespace DeskReservationApp.Domain.Entities
{
    /// <summary>
    /// Domain entity representing a desk reservation
    /// </summary>
    public class Reservation
    {
        public int ReservationId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int DeskId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; } = "Active"; // Active, Cancelled, Completed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Desk Desk { get; set; } = null!;
    }
}
