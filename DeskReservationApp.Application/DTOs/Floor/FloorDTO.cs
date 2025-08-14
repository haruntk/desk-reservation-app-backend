using DeskReservationApp.Application.DTOs.Desk;

namespace DeskReservationApp.Application.DTOs.Floor
{
    /// <summary>
    /// Floor data transfer object
    /// </summary>
    public class FloorDTO
    {
        public int FloorId { get; set; }
        public int FloorNumber { get; set; }
        public List<DeskDTO>? Desks { get; set; }
    }
}
