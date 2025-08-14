using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Authentication
{
    /// <summary>
    /// Request DTO for user registration
    /// </summary>
    public class RegisterRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
