using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Desk
{
    /// <summary>
    /// Request DTO for updating a desk
    /// </summary>
    public class UpdateDeskRequestDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Desk name must be between 1 and 50 characters")]
        public string DeskName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Floor ID must be a positive number")]
        public int FloorId { get; set; }
    }
}
