using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Reservation
{
    /// <summary>
    /// Request DTO for updating a reservation
    /// </summary>
    public class UpdateReservationRequestDTO : IValidatableObject
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Desk ID must be a positive number")]
        public int DeskId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (StartTime >= EndTime)
            {
                results.Add(new ValidationResult("End time must be after start time.", new[] { nameof(EndTime) }));
            }

            // Prevent updating reservations that have already started
            if (StartTime < DateTime.UtcNow)
            {
                results.Add(new ValidationResult("Cannot update a reservation that has already started.", new[] { nameof(StartTime) }));
            }

            return results;
        }
    }
}
