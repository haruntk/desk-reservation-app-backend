using DeskReservationApp.Application.DTOs.Floor;
using DeskReservationApp.Application.DTOs.Reservation;

namespace DeskReservationApp.Application.DTOs.Desk
{
    /// <summary>
    /// Desk data transfer object
    /// </summary>
    public class DeskDTO
    {
        public int DeskId { get; set; }
        public string DeskName { get; set; } = string.Empty;
        public int FloorId { get; set; }
        public FloorDTO? Floor { get; set; }
        public List<ReservationDTO>? Reservations { get; set; }
    }
}
