using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Floor
{
    /// <summary>
    /// Request DTO for updating a floor
    /// </summary>
    public class UpdateFloorRequestDTO
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Floor number must be between 1 and 100")]
        public int FloorNumber { get; set; }
    }
}
