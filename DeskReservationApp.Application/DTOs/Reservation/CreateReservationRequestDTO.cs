using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Reservation
{
    /// <summary>
    /// Request DTO for creating a new reservation
    /// </summary>
    public class CreateReservationRequestDTO : IValidatableObject
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

            if (StartTime < DateTime.UtcNow.AddMinutes(-5)) // Allow 5 minutes tolerance
            {
                results.Add(new ValidationResult("Start time cannot be in the past.", new[] { nameof(StartTime) }));
            }

            return results;
        }
    }
}
