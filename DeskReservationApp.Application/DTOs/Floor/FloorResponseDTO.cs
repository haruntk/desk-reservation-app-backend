namespace DeskReservationApp.Application.DTOs.Floor
{
    /// <summary>
    /// Response DTO for floor operations
    /// </summary>
    public class FloorResponseDTO
    {
        public int FloorId { get; set; }
        public int FloorNumber { get; set; }
        public int DeskCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
