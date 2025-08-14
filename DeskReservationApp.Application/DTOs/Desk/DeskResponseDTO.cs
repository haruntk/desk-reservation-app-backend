namespace DeskReservationApp.Application.DTOs.Desk
{
    /// <summary>
    /// Response DTO for desk operations
    /// </summary>
    public class DeskResponseDTO
    {
        public int DeskId { get; set; }
        public string DeskName { get; set; } = string.Empty;
        public int FloorId { get; set; }
        public string FloorNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime? NextReservationStart { get; set; }
    }
}
