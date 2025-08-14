using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Desk
{
    /// <summary>
    /// Request DTO for checking desk availability
    /// </summary>
    public class DeskAvailabilityRequestDTO
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
