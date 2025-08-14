using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Reservation
{
    /// <summary>
    /// Request DTO for updating reservation status
    /// </summary>
    public class UpdateReservationStatusRequestDTO
    {
        [Required]
        [RegularExpression("^(Active|Cancelled|Completed)$", ErrorMessage = "Status must be Active, Cancelled, or Completed")]
        public string Status { get; set; } = string.Empty;
    }
}
