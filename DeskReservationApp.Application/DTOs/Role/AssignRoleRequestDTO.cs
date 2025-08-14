using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Role
{
    /// <summary>
    /// Request DTO for assigning role to user
    /// </summary>
    public class AssignRoleRequestDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
