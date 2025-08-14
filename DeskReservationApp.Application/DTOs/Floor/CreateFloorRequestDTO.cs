using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Floor
{
    /// <summary>
    /// Request DTO for creating a new floor
    /// </summary>
    public class CreateFloorRequestDTO
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Floor number must be between 1 and 100")]
        public int FloorNumber { get; set; }
    }
}
