namespace DeskReservationApp.Domain.Entities
{
    /// <summary>
    /// Domain entity representing a floor in the building
    /// </summary>
    public class Floor
    {
        public int FloorId { get; set; }

        public int FloorNumber { get; set; }

        // Navigation properties
        public virtual ICollection<Desk> Desks { get; set; } = new List<Desk>();
    }
}
