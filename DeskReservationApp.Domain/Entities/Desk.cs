namespace DeskReservationApp.Domain.Entities
{
    /// <summary>
    /// Domain entity representing a desk in the office
    /// </summary>
    public class Desk
    {
        public int DeskId { get; set; }

        public string DeskName { get; set; } = string.Empty;

        public int FloorId { get; set; }

        // Navigation properties
        public virtual Floor Floor { get; set; } = null!;

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
